using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Messerli.CommandLine;
using Messerli.MetaGenerator.UserInput;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Messerli.TfsClient;
using Messerli.ToolLoader;
using Messerli.VsSolution;
using Stubble.Core.Builders;
using static Messerli.MetaGenerator.ExecutableInformation;

namespace Messerli.MetaGenerator
{
    internal class CompositionRoot
    {
        private readonly ContainerBuilder _builder = new ContainerBuilder();

        private CompositionRoot()
        {
        }

        public static CompositionRoot Create()
        {
            return new CompositionRoot();
        }

        public IContainer Build()
        {
            return _builder.Build();
        }

        public CompositionRoot RegisterGenerator()
        {
            _builder.RegisterType<Application>().As<IApplication>();

            _builder.RegisterType<ValidatedUserInput>().As<IValidatedUserInput>();
            _builder.RegisterType<UserInputProvider>().As<IUserInputProvider>().SingleInstance();
            _builder.RegisterType<VariableProvider>().As<IVariableProvider>().SingleInstance();

            _builder.RegisterType<UserInputDescriptionBuilder>().AsSelf();
            _builder.RegisterType<TemplateLoader>().As<ITemplateLoader>();
            _builder.RegisterType<TimeKeeper>().As<ITimeKeeper>();

            _builder.RegisterType<ConsoleTfsClient>().As<ITfsClient>();
            _builder.RegisterType<FileGenerator>().As<IFileGenerator>();
            _builder.RegisterType<FileManipulator>().As<IFileManipulator>();
            _builder.RegisterType<StubbleBuilder>().AsSelf();

            _builder.RegisterType<ExecutingPluginAssemblyProvider>().As<IExecutingPluginAssemblyProvider>().SingleInstance();

            RegisterVariableRequesters();

            return this;
        }

        public CompositionRoot RegisterModules()
        {
            _builder.RegisterModule<CommandLineModule>();
            _builder.RegisterModule<ToolLoaderModule>();
            _builder.RegisterModule<VsSolutionModule>();

            return this;
        }

        public CompositionRoot RegisterPlugins()
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
                            var registrar = _builder.RegisterAssemblyModules(assembly);
                        }
                    });

            return this;
        }

        private void RegisterVariableRequesters()
        {
            _builder.RegisterType<StringRequester>();
            _builder.RegisterType<BoolRequester>();
            _builder.RegisterType<IntegerRequester>();
            _builder.RegisterType<DoubleRequester>();
            _builder.RegisterType<SelectionRequester>();
            _builder.RegisterType<PathRequester>();
            _builder.RegisterType<ExistingPathRequester>();
            _builder.RegisterType<DateRequester>();
            _builder.RegisterType<DateTimeRequester>();
            _builder.RegisterType<TimeRequester>();

            _builder.Register(VariableRequesterFactory).As<IVariableRequester>();
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