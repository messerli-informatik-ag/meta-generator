using System;
using System.IO;
using System.Reflection;
using Autofac;
using Messerli.CommandLine;
using Messerli.FileManipulator;
using Messerli.FileManipulatorAbstractions;
using Messerli.MetaGenerator.UserInput;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
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
            _builder.RegisterType<RootCommandBuilder>().As<IRootCommandBuilder>();
            _builder.RegisterType<GeneratorCommandBuilder>().As<IGeneratorCommandBuilder>();
            _builder.RegisterType<PluginSelection>().As<IPluginSelection>().SingleInstance();
            _builder.RegisterType<PluginManager>().As<IPluginManager>();

            _builder.RegisterType<GenerationSteps>().As<IGenerationSteps>();
            _builder.RegisterType<ValidatedUserInput>().As<IValidatedUserInput>();
            _builder.RegisterType<UserInputProvider>().As<IUserInputProvider>().InstancePerLifetimeScope();
            _builder.RegisterType<VariableProvider>().As<IVariableProvider>().InstancePerLifetimeScope();

            _builder.RegisterType<UserInputDescriptionBuilder>().AsSelf();
            _builder.RegisterType<TemplateLoader>().As<ITemplateLoader>();
            _builder.RegisterType<TimeKeeper>().As<ITimeKeeper>();

            _builder.RegisterType<FileGenerator>().As<IFileGenerator>();
            _builder.RegisterType<FileManipulator>().As<IFileManipulator>();
            _builder.RegisterType<GlobalJsonManipulator>().As<IGlobalJsonManipulator>();
            _builder.RegisterType<StubbleBuilder>().AsSelf();

            _builder.RegisterType<ExceptionFormatter>().As<IExceptionFormatter>().InstancePerLifetimeScope();

            _builder.RegisterType<ExecutingPluginAssemblyProvider>().As<IExecutingPluginAssemblyProvider>().InstancePerLifetimeScope();

            _builder.RegisterType<DotnetToolInstaller>().As<IDotnetToolInstaller>();

            RegisterVariableRequesters();

            return this;
        }

        public CompositionRoot RegisterModules()
        {
            _builder.RegisterModule<CommandLineModule>();
            _builder.RegisterModule<ToolLoaderModule>();
            _builder.RegisterModule<VsSolutionModule>();
            _builder.RegisterModule<FileManipulatorModule>();

            return this;
        }

        public CompositionRoot RegisterPlugins()
        {
            var pluginsPath = CreateFolderWhenNecessary(Path.Combine(GetExecutableDirectory(), "plugins"));

            foreach (var pluginPath in Directory.GetDirectories(pluginsPath, "*"))
            {
                var pluginName = Path.GetRelativePath(pluginsPath, pluginPath);
                var pluginDllPath = Path.Combine(pluginPath, $"{pluginName}.dll");
                var loadContext = new PluginLoadContext(pluginDllPath);

                var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginDllPath)));
                _ = _builder.RegisterAssemblyModules(assembly);
            }

            return this;
        }

        public CompositionRoot RegisterGlobalOptions(bool verbose)
        {
            _builder.Register(_ => new GlobalOptions(verbose)).As<IGlobalOptions>().SingleInstance();

            return this;
        }

        private string CreateFolderWhenNecessary(string pluginsPath)
        {
            if (Directory.Exists(pluginsPath) == false)
            {
                Directory.CreateDirectory(pluginsPath);
            }

            return pluginsPath;
        }

        private void RegisterVariableRequesters()
        {
            _builder.RegisterType<StringRequester>().Keyed<AbstractVariableRequester>(VariableType.String);
            _builder.RegisterType<BoolRequester>().Keyed<AbstractVariableRequester>(VariableType.Bool);
            _builder.RegisterType<IntegerRequester>().Keyed<AbstractVariableRequester>(VariableType.Integer);
            _builder.RegisterType<DoubleRequester>().Keyed<AbstractVariableRequester>(VariableType.Double);
            _builder.RegisterType<SelectionRequester>().Keyed<AbstractVariableRequester>(VariableType.Selection).AsSelf();
            _builder.RegisterType<PathRequester>().Keyed<AbstractVariableRequester>(VariableType.Path);
            _builder.RegisterType<ExistingPathRequester>().Keyed<AbstractVariableRequester>(VariableType.ExistingPath);
            _builder.RegisterType<DateRequester>().Keyed<AbstractVariableRequester>(VariableType.Date);
            _builder.RegisterType<DateTimeRequester>().Keyed<AbstractVariableRequester>(VariableType.DateTime);
            _builder.RegisterType<TimeRequester>().Keyed<AbstractVariableRequester>(VariableType.Time);
        }
    }
}
