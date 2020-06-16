using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Autofac;
using Funcky.Extensions;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator
{
    internal class RootCommandBuilder : IRootCommandBuilder
    {
        private readonly ILifetimeScope _programScope;
        private readonly IEnumerable<IMetaGenerator> _generators;
        private readonly IPluginSelection _pluginSelection;
        private readonly IPluginManager _pluginManager;

        public RootCommandBuilder(
            ILifetimeScope programScope,
            IEnumerable<IMetaGenerator> generators,
            IPluginSelection pluginSelection,
            IPluginManager pluginManager)
        {
            _programScope = programScope;
            _generators = generators;
            _pluginSelection = pluginSelection;
            _pluginManager = pluginManager;
        }

        public RootCommand Build()
        {
            var root = new RootCommand("The Messerli meta-generator is a versatile generator which can create files, projects, repositories with the right plugins!")
            {
                Handler = CommandHandler.Create<InvocationContext>(HandleContext),
            };

            root.AddGlobalOption(VerboseOption());

            RegisterPluginCommands(root);

            root.AddCommand(CreatePluginManagerCommands());

            return root;
        }

        private void HandleContext(InvocationContext context)
        {
            context.ResultCode = _pluginSelection.StartPluginInteractive(context);
        }

        private Option VerboseOption()
        {
            var option = new Option<bool>("--verbose", "More verbose output, including stack traces");

            option.AddAlias("-v");

            return option;
        }

        private void RegisterPluginCommands(Command root)
        {
            _generators
                .Select(CreateCommandInLifeTimeScope)
                .ForEach(o => o.AndThen(c => root.Add(c)));
        }

        private Funcky.Monads.Option<Command> CreateCommandInLifeTimeScope(IMetaGenerator generator)
        {
            using var scope = _programScope.BeginLifetimeScope();

            try
            {
                var result = scope
                    .Resolve<IGeneratorCommandBuilder>()
                    .Build(generator.Name);

                return Funcky.Monads.Option.Some(result);
            }
            catch (Exception exception)
            {
                scope
                    .Resolve<IExceptionFormatter>()
                    .FormatException(exception);

                return Funcky.Monads.Option<Command>.None();
            }
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
    }
}
