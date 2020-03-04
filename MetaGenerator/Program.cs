using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            using var container = CompositionRoot
                    .Create()
                    .RegisterModules()
                    .RegisterGenerator()
                    .RegisterPlugins()
                    .Build();

            return container
                .Resolve<IApplication>()
                .Run(args);
        }
    }
}
