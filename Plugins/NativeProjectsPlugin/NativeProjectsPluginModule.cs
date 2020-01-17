using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.NativeProjectsPlugin
{
    public class NativeProjectsPluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NativeProjectsPluginGenerator>().As<IMetaGenerator>();
        }
    }
}