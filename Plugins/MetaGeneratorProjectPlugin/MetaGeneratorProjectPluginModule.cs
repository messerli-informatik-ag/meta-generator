using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGeneratorProjectPlugin;

public class MetaGeneratorProjectPluginModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MetaGeneratorProjectPluginGenerator>().As<IMetaGenerator>().InstancePerLifetimeScope();
    }
}
