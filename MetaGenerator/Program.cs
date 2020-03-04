using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            using var rootContainer = CompositionRoot
                    .Create()
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
    }
}
