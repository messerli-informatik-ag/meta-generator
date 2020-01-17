using Autofac;
using Messerli.ProjectAbstractions;

namespace Messerli.ProjectGeneratorPluginProjects
{
    public class ProjectGeneratorPluginProjectsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjectGeneratorPluginProjectsGenerator>().As<IProjectGenerator>();
        }
    }
}