using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Funcky.Extensions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator
{
    internal class GeneratorCommandBuilder : IGeneratorCommandBuilder
    {
        private readonly IEnumerable<IMetaGenerator> _generators;
        private readonly IPluginSelection _pluginSelection;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IExecutingPluginAssemblyProvider _executingPluginAssemblyProvider;

        public GeneratorCommandBuilder(
            IEnumerable<IMetaGenerator> generators,
            IPluginSelection pluginSelection,
            IUserInputProvider userInputProvider,
            IExecutingPluginAssemblyProvider executingPluginAssemblyProvider)
        {
            _generators = generators;
            _pluginSelection = pluginSelection;
            _userInputProvider = userInputProvider;
            _executingPluginAssemblyProvider = executingPluginAssemblyProvider;
        }

        public Command Build(string commandName)
        {
            return CreateGeneratorCommands(RegisterPluginVariables(_generators.First(generator => generator.Name == commandName)));
        }

        private Command CreateGeneratorCommands(IMetaGenerator generator)
        {
            var command = new Command(generator.Name, generator.Description)
            {
                Handler = CommandHandler.Create<InvocationContext>(context => { HandleContext(context, generator); }),
            };

            _userInputProvider.GetUserInputDescriptions().ForEach(variable => command.AddOption(CreateOption(variable)));

            return command;
        }

        private void HandleContext(InvocationContext context, IMetaGenerator generator)
        {
            context.ResultCode = _pluginSelection.StartPlugin(context, generator.Name);
        }

        private IMetaGenerator RegisterPluginVariables(IMetaGenerator generator)
        {
            _executingPluginAssemblyProvider.PluginAssembly = generator.GetType().Assembly;

            generator.Register();

            return generator;
        }

        private static Option CreateOption(IUserInputDescription userInput)
        {
            var option = new Option(UserOptionFormat.ToUserOption(userInput.VariableName), userInput.VariableDescription.GetOrElse(string.Empty))
            {
                Argument = new Argument<string>(),
            };

            return option;
        }
    }
}
