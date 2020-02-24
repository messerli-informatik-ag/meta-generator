using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.NonInteractivePlugin
{
    public class NonInteractivePluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NonInteractivePluginGenerator>().As<IMetaGenerator>();
        }
    }
}