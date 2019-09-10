using Autofac;
using Messerli.ProjectAbstractions;

namespace Messerli.NativeProjects
{
    public class NativeProjectsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NativeTestProjectGenerator>().As<IProjectGenerator>();
        }
    }
}
