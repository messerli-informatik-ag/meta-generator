using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGenerator.UserInput;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator
{
    internal class Application : IApplication
    {
        private readonly IConsoleWriter _consoleWriter;
        private readonly IEnumerable<IMetaGenerator> _generators;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IExecutingPluginAssemblyProvider _assemblyProvider;
        private readonly RootCommand _rootCommand;
        private readonly SelectionRequester _selectionRequester;
        private readonly Func<UserInputDescriptionBuilder> _newNewUserInputDescriptionBuilder;
        private readonly ITimeKeeper _timeKeeper;

        public Application(
            IConsoleWriter consoleWriter,
            IEnumerable<IMetaGenerator> generators,
            IUserInputProvider userInputProvider,
            IExecutingPluginAssemblyProvider assemblyProvider,
            SelectionRequester selectionRequester,
            ITimeKeeper timeKeeper,
            Func<UserInputDescriptionBuilder> newUserInputDescriptionBuilder)
        {
            _consoleWriter = consoleWriter;
            _generators = generators;
            _userInputProvider = userInputProvider;
            _assemblyProvider = assemblyProvider;
            _selectionRequester = selectionRequester;
            _newNewUserInputDescriptionBuilder = newUserInputDescriptionBuilder;

            _rootCommand = SetupRootCommand();
            SetupGeneratorCommand();
            _timeKeeper = timeKeeper;
        }

        public int Run(string[] args)
        {
            return _rootCommand.Invoke(args);
        }

        private RootCommand SetupRootCommand()
        {
            var rootCommand = new RootCommand
            {
                Description = "This is the meta generator for the automatic creation of files, wrappers, projects or whatever the plugin provides.",
                Handler = CommandHandler.Create<string>(ExecuteWizard),
            };

            var generatorTypeOption = new Option("--generator-type", "Give the generator-type")
            {
                Argument = new Argument<string?>(defaultValue: () => null),
            };

            rootCommand.AddOption(generatorTypeOption);

            return rootCommand;
        }

        private void SetupGeneratorCommand()
        {
            var generatorCommand = new Command("generators", "List all generator types")
            {
                Handler = CommandHandler.Create(ListGenerators),
            };

            _rootCommand.AddCommand(generatorCommand);
        }

        private void ListGenerators()
        {
            _consoleWriter.WriteLine("Available generator-plugins");
            _consoleWriter.WriteLine();

            if (_generators.Any())
            {
                foreach (var generator in _generators)
                {
                    _consoleWriter.WriteLine($"* {generator.ShortName} ({generator.Name})");
                }
            }
            else
            {
                _consoleWriter.WriteLine("No generator plugins found.");
            }
        }

        private void ExecuteWizard(string selectedGenerator)
        {
            _consoleWriter.WriteLine("Welcome to the meta-generator wizard");

            var metaGenerator = _generators
                .FirstOrDefault(generator => generator.ShortName == selectedGenerator)
                ?? _selectionRequester
                    .RequestValue(ToSelection(_generators))
                    .AndThen(shortName => _generators
                        .FirstOrDefault(generator => generator.ShortName == shortName))
                    .OrElse(NullMetaGenerator.Instance);

            ExecuteGenerator(metaGenerator);
        }

        private IUserInputDescription ToSelection(IEnumerable<IMetaGenerator> metaGenerators)
        {
            var builder = _newNewUserInputDescriptionBuilder();

            builder.SetVariableName("GeneratorType");
            builder.SetVariableDescription("What do you want to generate?");
            builder.SetVariableType(VariableType.Selection);
            builder.SetVariableQuestion("What do you want to generate? Please select from the following options.");
            builder.SetSelectionValues(ToSelectionValues(metaGenerators));

            return builder.Build();
        }

        private List<SelectionValue> ToSelectionValues(IEnumerable<IMetaGenerator> metaGenerators)
        {
            return metaGenerators
                .Select(metaGenerator => new SelectionValue { Value = metaGenerator.Name, Description = metaGenerator.ShortName })
                .ToList();
        }

        private void ExecuteGenerator(IMetaGenerator metaTypeGenerator)
        {
            _assemblyProvider.PluginAssembly = metaTypeGenerator.GetType().Assembly;

            _timeKeeper.MeasureTime(metaTypeGenerator.Register, "Registration");

            _userInputProvider.AskUser();

            _timeKeeper.MeasureTime(metaTypeGenerator.Prepare, "Prepartion");
            _timeKeeper.MeasureTime(metaTypeGenerator.Generate, "Generation");
            _timeKeeper.MeasureTime(metaTypeGenerator.TearDown, "Tear down");

            _timeKeeper.Print();
        }
    }
}