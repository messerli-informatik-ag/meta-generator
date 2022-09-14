using Autofac;
using Messerli.CommandLineAbstractions;

namespace Messerli.CommandLine;

public class CommandLineModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<SystemConsoleWriter>().As<IConsoleWriter>();
        builder.RegisterType<SystemConsoleReader>().As<IConsoleReader>();
    }
}
