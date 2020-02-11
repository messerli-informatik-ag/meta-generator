using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MesserliOneRepositoryPlugin
{
    public class MesserliOneRepositoryPluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MesserliOneRepositoryPluginGenerator>().As<IMetaGenerator>();
        }
    }
}