using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;
using Messerli.ProjectAbstractions.Json;
using Messerli.ProjectAbstractions.UserInput;
using Messerli.ProjectGenerator.UserInput;

namespace Messerli.ProjectGenerator
{
    internal class Application : IApplication
    {
        private readonly IConsoleWriter _consoleWriter;
        private readonly IEnumerable<IProjectGenerator> _projectGenerators;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IExecutingPluginAssemblyProvider _assemblyProvider;
        private readonly RootCommand _rootCommand;
        private readonly SelectionRequester _selectionRequester;
        private readonly Func<UserInputDescriptionBuilder> _newNewUserInputDescriptionBuilder;

        public Application(
            IConsoleWriter consoleWriter,
            IEnumerable<IProjectGenerator> projectGenerators,
            IUserInputProvider userInputProvider,
            IExecutingPluginAssemblyProvider assemblyProvider,
            SelectionRequester selectionRequester,
            Func<UserInputDescriptionBuilder> newUserInputDescriptionBuilder)
        {
            _consoleWriter = consoleWriter;
            _projectGenerators = projectGenerators;
            _userInputProvider = userInputProvider;
            _assemblyProvider = assemblyProvider;
            _selectionRequester = selectionRequester;
            _newNewUserInputDescriptionBuilder = newUserInputDescriptionBuilder;

            _rootCommand = SetupRootCommand();
            SetupProjectCommand();
        }

        public int Run(string[] args)
        {
            return _rootCommand.Invoke(args);
        }

        private RootCommand SetupRootCommand()
        {
            var rootCommand = new RootCommand
            {
                Description = "This is the Messerli Project generator",
                Handler = CommandHandler.Create<string>(ExecuteWizard),
            };

            var projectTypeOption = new Option("--project-type", "Give the project-type")
            {
                Argument = new Argument<string?>(defaultValue: () => null),
            };

            rootCommand.AddOption(projectTypeOption);

            return rootCommand;
        }

        private void SetupProjectCommand()
        {
            var projectsCommand = new Command("projects", "List all project types")
            {
                Handler = CommandHandler.Create(ListProjects),
            };

            _rootCommand.AddCommand(projectsCommand);
        }

        private void ListProjects()
        {
            _consoleWriter.WriteLine($"Available project types");

            foreach (var projectGenerator in _projectGenerators)
            {
                _consoleWriter.WriteLine($"* {projectGenerator.ShortName}");
            }
        }

        private void ExecuteWizard(string projectType)
        {
            _consoleWriter.WriteLine("Welcome to the project generator wizard: ");
            _consoleWriter.WriteLine(string.Empty);

            var projectGenerator = _projectGenerators
                .FirstOrDefault(generator => generator.ShortName == projectType)
                ?? _selectionRequester
                    .RequestValue(ToSelection(_projectGenerators))
                    .AndThen(shortName => _projectGenerators
                        .FirstOrDefault(generator => generator.ShortName == shortName))
                    .OrElse(NullProjectGenerator.Instance);

            ExecuteProjectGenerator(projectGenerator);
        }

        private IUserInputDescription ToSelection(IEnumerable<IProjectGenerator> projectGenerators)
        {
            var builder = _newNewUserInputDescriptionBuilder();

            builder.SetVariableName("ProjectType");
            builder.SetVariableDescription("The type of the project");
            builder.SetVariableType(VariableType.Selection);
            builder.SetVariableQuestion("What kind of project do you want to generate? Please select from the following options.");
            builder.SetSelectionValues(ToSelectionValues(projectGenerators));

            return builder.Build();
        }

        private List<SelectionValue> ToSelectionValues(IEnumerable<IProjectGenerator> projectGenerators)
        {
            return projectGenerators
                .Select(projectGenerator => new SelectionValue { Value = projectGenerator.ShortName, Description = projectGenerator.Name })
                .ToList();
        }

        private void ExecuteProjectGenerator(IProjectGenerator projectTypeGenerator)
        {
            _assemblyProvider.PluginAssembly = projectTypeGenerator.GetType().Assembly;

            projectTypeGenerator.Register();
            _userInputProvider.AskUser();
            projectTypeGenerator.Generate();
        }
    }
}