using Autofac;
using Messerli.ProjectAbstractions;

namespace Messerli.ProjectGenerator
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            using (var container = new CompositionRoot().Build())
            {
                var application = container.Resolve<IApplication>();

                return application.Run(args);
            }
        }
    }
}
