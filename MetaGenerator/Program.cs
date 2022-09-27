using System.Collections.Generic;
using System.Linq;
using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator;

internal class Program
{
    public static int Main(string[] args)
    {
        using var rootContainer = CompositionRoot
            .Create()
            .RegisterGlobalOptions(IsVerbose(args))
            .RegisterModules()
            .RegisterGenerator()
            .RegisterPlugins()
            .Build();

        // Autofac documentation advices to resolve from a lifetimescope instead of the root container
        using var programLifetime = rootContainer.BeginLifetimeScope();

        return programLifetime
            .Resolve<IApplication>()
            .Run(args);
    }

    private static bool IsVerbose(IEnumerable<string> arguments)
    {
        // we parse the verbose option here by hand and circumvent System.CommandLine because we need the verbosity already during the Setup of System.CommandLine
        return arguments.Any(a => a == "--verbose" || a == "-v");
    }
}
