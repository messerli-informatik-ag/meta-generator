using Autofac;
using Messerli.MetaGeneratorAbstractions;
using Messerli.TfsClient;

namespace Messerli.NativeProjectsPlugin
{
    internal class NativeProjectsPluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NativeProjectsPluginGenerator>().As<IMetaGenerator>();
            builder.RegisterType<TfsPaths>().As<ITfsPaths>();
            builder.RegisterType<ConsoleTfsClient>().As<ITfsClient>();
            builder.RegisterType<ProjectInformation>().As<IProjectInformation>().SingleInstance();
        }
    }
}