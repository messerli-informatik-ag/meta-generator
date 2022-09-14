using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.NativeProjectsPlugin;

internal class NativeProjectsPluginModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<NativeProjectsPluginGenerator>().As<IMetaGenerator>().InstancePerLifetimeScope();
        builder.RegisterType<TfsPaths>().As<ITfsPaths>();
        builder.RegisterType<ProjectInformation>().As<IProjectInformation>().SingleInstance();
    }
}
