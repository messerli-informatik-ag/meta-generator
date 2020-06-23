using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.BackbonePluginTemplatePlugin
{
    public class BackbonePluginTemplatePluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BackbonePluginTemplatePluginGenerator>().As<IMetaGenerator>().InstancePerLifetimeScope();
        }
    }
}