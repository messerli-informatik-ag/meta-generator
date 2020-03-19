using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.ManagedWrapperProjectsPlugin
{
    public class ManagedWrapperProjectsPluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ManagedWrapperProjectsPluginGenerator>().As<IMetaGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<PathProvider>().As<IPathProvider>().InstancePerLifetimeScope();
        }
    }
}