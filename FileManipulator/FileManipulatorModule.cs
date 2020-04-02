using Autofac;
using Messerli.FileManipulator.Project;
using Messerli.FileManipulatorAbstractions;
using Messerli.FileManipulatorAbstractions.Project;
using Messerli.FileSystem;

namespace Messerli.FileManipulator
{
    public class FileManipulatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GlobalJsonManipulator>().As<IGlobalJsonManipulator>();
            builder.RegisterType<FileOpeningBuilder>().As<IFileOpeningBuilder>();
            builder.RegisterType<MsBuildProjectManipulatorFacade>().As<IProjectManipulator>();
            builder.RegisterType<Project.MsBuild.MicrosoftBuildAssemblyLoader>().As<Project.MsBuild.IMicrosoftBuildAssemblyLoader>();
            builder.RegisterType<Project.MsBuild.ProjectManipulator>().As<Project.MsBuild.IProjectManipulator>();
            builder.RegisterType<Project.MsBuild.ProjectSdkManipulator>().As<Project.MsBuild.IProjectSdkManipulator>();
            builder.RegisterType<Project.MsBuild.CentralPackageVersionsManipulator>().As<Project.MsBuild.ICentralPackageVersionsManipulator>();
        }
    }
}