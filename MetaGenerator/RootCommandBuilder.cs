using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text.RegularExpressions;
using Funcky.Extensions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator
{
    internal class RootCommandBuilder : IRootCommandBuilder
    {
        private readonly IEnumerable<IMetaGenerator> _generators;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IExecutingPluginAssemblyProvider _executingPluginAssemblyProvider;
        private readonly IPluginSelection _pluginSelection;

        public RootCommandBuilder(IEnumerable<IMetaGenerator> generators, IUserInputProvider userInputProvider, IExecutingPluginAssemblyProvider executingPluginAssemblyProvider, IPluginSelection pluginSelection)
        {
            _generators = generators;
            _userInputProvider = userInputProvider;
            _executingPluginAssemblyProvider = executingPluginAssemblyProvider;
            _pluginSelection = pluginSelection;
        }

        public RootCommand Build()
        {
            var root = new RootCommand("The Messerli meta-generator is a versatile generator which can create files, projects, repositories with the right plugins!")
            {
                Handler = CommandHandler.Create<InvocationContext>(context => { _pluginSelection.StartPluginInteractive(context); }),
            };

            _generators.Each(generator => root.AddCommand(CreateCommands(generator)));

            return root;
        }

        private Command CreateCommands(IMetaGenerator generator)
        {
            var command = new Command(generator.Name, generator.Description)
            {
                Handler = CommandHandler.Create<InvocationContext>(context => { _pluginSelection.StartPlugin(context, generator.Name); }),
            };

            _executingPluginAssemblyProvider.PluginAssembly = generator.GetType().Assembly;
            generator.Register();
            _userInputProvider.GetUserInputDescriptions().Each(variable => command.AddOption(CreateOption(variable)));
            _executingPluginAssemblyProvider.Clear();
            _userInputProvider.Clear();

            return command;
        }

        private static Option CreateOption(IUserInputDescription userInput)
        {
            var option = new Option(UserUptionFormat.ToUserOption(userInput.VariableName), userInput.VariableDescription)
            {
                Argument = new Argument<string>(),
            };

            return option;
        }
    }
}
