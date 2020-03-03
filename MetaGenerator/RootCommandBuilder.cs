using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
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
        private readonly IPluginManager _pluginManager;

        public RootCommandBuilder(
            IEnumerable<IMetaGenerator> generators,
            IUserInputProvider userInputProvider,
            IExecutingPluginAssemblyProvider executingPluginAssemblyProvider,
            IPluginSelection pluginSelection,
            IPluginManager pluginManager)
        {
            _generators = generators;
            _userInputProvider = userInputProvider;
            _executingPluginAssemblyProvider = executingPluginAssemblyProvider;
            _pluginSelection = pluginSelection;
            _pluginManager = pluginManager;
        }

        public RootCommand Build()
        {
            var root = new RootCommand("The Messerli meta-generator is a versatile generator which can create files, projects, repositories with the right plugins!")
            {
                Handler = CommandHandler.Create<InvocationContext>(context => { context.ResultCode = _pluginSelection.StartPluginInteractive(context); }),
            };

            _generators.Each(generator => root.AddCommand(CreateGeneratorCommands(generator)));

            root.AddCommand(CreatePluginManagerCommands());

            return root;
        }

        private Command CreatePluginManagerCommands()
        {
            var pluginCommand = new Command("plugin", "Plugin manager");

            pluginCommand.AddCommand(CreateInstallPluginCommand());
            pluginCommand.AddCommand(CreateUninstallPluginCommand());

            return pluginCommand;
        }

        private Command CreateInstallPluginCommand()
        {
            var installCommand = new Command("install", "Install a plugin from the plugin repository")
            {
                Handler = CommandHandler.Create<string>(_pluginManager.Install),
            };

            installCommand.AddArgument(new Argument { Arity = new ArgumentArity(1, 1), Name = "pluginName", Description = "The name of the plugin you want to install." });

            return installCommand;
        }

        private Command CreateUninstallPluginCommand()
        {
            var uninstallCommand = new Command("uninstall", "Install a plugin from the plugin repository")
            {
                Handler = CommandHandler.Create<string>(_pluginManager.Uninstall),
            };

            uninstallCommand.AddArgument(new Argument { Arity = new ArgumentArity(1, 1), Name = "pluginName", Description = "The name of the plugin you want to uninstall." });

            return uninstallCommand;
        }

        private Command CreateGeneratorCommands(IMetaGenerator generator)
        {
            var command = new Command(generator.Name, generator.Description)
            {
                Handler = CommandHandler.Create<InvocationContext>(context => { context.ResultCode = _pluginSelection.StartPlugin(context, generator.Name); }),
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
