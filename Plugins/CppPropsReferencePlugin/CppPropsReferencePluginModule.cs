using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.CppPropsReferencePlugin
{
    public class CppPropsReferencePluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CppPropsReferencePluginGenerator>().As<IMetaGenerator>().InstancePerLifetimeScope();
        }
    }
}