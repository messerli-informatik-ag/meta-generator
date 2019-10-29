using Autofac;
using Messerli.CommandLine;
using Messerli.CommandLineAbstractions;
using Messerli.NativeProjects;
using Messerli.OneCoreProjects;
using Messerli.ProjectAbstractions;
using Messerli.TfsClient;
using Stubble.Core.Builders;

namespace Messerli.ProjectGenerator
{
    internal class CompositionRoot
    {
        public IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Application>().As<IApplication>();

            builder.RegisterType<SystemConsoleWriter>().As<IConsoleWriter>();
            builder.RegisterType<SystemConsoleReader>().As<IConsoleReader>();
            builder.RegisterType<UserInputProvider>().As<IUserInputProvider>().SingleInstance();

            builder.RegisterType<ConsoleClient>().As<IClient>();
            builder.RegisterType<FileGenerator>().As<IFileGenerator>();
            builder.RegisterType<StubbleBuilder>().AsSelf();

            builder.RegisterModule<NativeProjectsModule>();
            builder.RegisterModule<OneCoreProjectsModule>();

            return builder.Build();
        }
    }
}