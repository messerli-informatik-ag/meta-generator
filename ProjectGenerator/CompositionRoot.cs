using Autofac;
using Messerli.CommandLine;
using Messerli.CommandLineAbstractions;
using Messerli.NativeProjects;
using Messerli.OneCoreProjects;
using Messerli.TfsClient;

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

            builder.RegisterType<ConsoleClient>().As<IClient>();

            builder.RegisterModule<NativeProjectsModule>();
            builder.RegisterModule<OneCoreProjectsModule>();

            return builder.Build();
        }
    }
}