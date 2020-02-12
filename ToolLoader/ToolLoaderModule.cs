using Autofac;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.ToolLoader
{
    public class ToolLoaderModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Tools>().As<ITools>().SingleInstance();
            builder.RegisterType<Tool>().As<Tool>();
        }
    }
}
