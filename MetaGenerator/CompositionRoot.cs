﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Messerli.CommandLine;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGenerator.UserInput;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Messerli.TfsClient;
using Messerli.VsSolution;
using Stubble.Core.Builders;
using static Messerli.MetaGenerator.ExecutableInformation;

namespace Messerli.MetaGenerator
{
    internal class CompositionRoot
    {
        public IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Application>().As<IApplication>();

            builder.RegisterType<SystemConsoleWriter>().As<IConsoleWriter>();
            builder.RegisterType<SystemConsoleReader>().As<IConsoleReader>();
            builder.RegisterType<ValidatedUserInput>().As<IValidatedUserInput>();
            builder.RegisterType<UserInputProvider>().As<IUserInputProvider>().SingleInstance();
            builder.RegisterType<VariableProvider>().As<IVariableProvider>().SingleInstance();

            builder.RegisterType<UserInputDescriptionBuilder>().AsSelf();
            builder.RegisterType<TemplateLoader>().As<ITemplateLoader>();
            builder.RegisterType<TimeKeeper>().As<ITimeKeeper>();

            builder.RegisterType<ConsoleTfsClient>().As<ITfsClient>();
            builder.RegisterType<FileGenerator>().As<IFileGenerator>();
            builder.RegisterType<FileManipulator>().As<IFileManipulator>();
            builder.RegisterType<StubbleBuilder>().AsSelf();

            builder.RegisterType<ExecutingPluginAssemblyProvider>().As<IExecutingPluginAssemblyProvider>().SingleInstance();

            RegisterVariableRequesters(builder);

            builder.RegisterType<SolutionLoader>().As<ISolutionLoader>();

            RegisterPlugins(builder);

            return builder.Build();
        }

        private static void RegisterVariableRequesters(ContainerBuilder builder)
        {
            builder.RegisterType<StringRequester>();
            builder.RegisterType<BoolRequester>();
            builder.RegisterType<IntegerRequester>();
            builder.RegisterType<DoubleRequester>();
            builder.RegisterType<SelectionRequester>();
            builder.RegisterType<PathRequester>();
            builder.RegisterType<ExistingPathRequester>();
            builder.RegisterType<DateRequester>();
            builder.RegisterType<DateTimeRequester>();
            builder.RegisterType<TimeRequester>();

            builder.Register(VariableRequesterFactory).As<IVariableRequester>();
        }

        private void RegisterPlugins(ContainerBuilder builder)
        {
            GetExecutableDirectory()
                .Match(
                () => throw new Exception("Failed to get directory of executable."),
                executablePath =>
                {
                    var pluginsPath = Path.Combine(executablePath, "plugins");
                    foreach (var pluginPath in Directory.GetDirectories(pluginsPath, "*"))
                    {
                        var pluginName = Path.GetRelativePath(pluginsPath, pluginPath);
                        var pluginDllPath = Path.Combine(pluginPath, $"{pluginName}.dll");
                        var loadContext = new PluginLoadContext(pluginDllPath);

                        var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginDllPath)));
                        var registrar = builder.RegisterAssemblyModules(assembly);
                    }
                });
        }

        private static IVariableRequester VariableRequesterFactory(IComponentContext context, IEnumerable<Parameter> paremeter)
        {
            var variableType = paremeter.TypedAs<VariableType>();

            return variableType switch
            {
                VariableType.String => context.Resolve<StringRequester>(),
                VariableType.Bool => context.Resolve<BoolRequester>(),
                VariableType.Integer => context.Resolve<IntegerRequester>(),
                VariableType.Double => context.Resolve<DoubleRequester>(),
                VariableType.Selection => context.Resolve<SelectionRequester>(),
                VariableType.Path => context.Resolve<PathRequester>(),
                VariableType.ExistingPath => context.Resolve<ExistingPathRequester>(),
                VariableType.Date => context.Resolve<DateRequester>(),
                VariableType.DateTime => context.Resolve<DateTimeRequester>(),
                VariableType.Time => context.Resolve<TimeRequester>(),

                _ => throw new NotImplementedException($"The VariableType '{variableType}' is not supported at the moment..."),
            };
        }
    }
}