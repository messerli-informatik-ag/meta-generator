using Autofac;

namespace Messerli.VsSolution
{
    public class VsSolutionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SolutionLoader>().As<ISolutionLoader>();
        }
    }
}
