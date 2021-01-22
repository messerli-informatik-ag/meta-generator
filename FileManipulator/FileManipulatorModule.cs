using Autofac;
using Messerli.FileManipulator.Project;
using Messerli.FileManipulator.Project.MsBuild;
using Messerli.FileManipulatorAbstractions;
using Messerli.FileSystem;
using NuGet.Common;

namespace Messerli.FileManipulator
{
    public class FileManipulatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GlobalJsonManipulator>().As<IGlobalJsonManipulator>();
            builder.RegisterType<FileOpeningBuilder>().As<IFileOpeningBuilder>();
            builder.RegisterType<NugetConfigurationManipulator>().As<INugetConfigurationManipulator>();
            builder.RegisterType<NugetPackageSourceManipulator>().As<INugetPackageSourceManipulator>();
            builder.RegisterType<NullLogger>().As<ILogger>();
            builder.RegisterType<MsBuildProjectManipulatorFacade>().As<FileManipulatorAbstractions.Project.IProjectManipulator>();
            builder.RegisterType<MicrosoftBuildAssemblyLoader>().As<IMicrosoftBuildAssemblyLoader>();
            builder.RegisterType<ProjectManipulator>().As<Project.MsBuild.IProjectManipulator>();
            builder.RegisterType<ProjectSdkManipulator>().As<IProjectSdkManipulator>();
            builder.RegisterType<CentralPackageVersionsManipulator>().As<ICentralPackageVersionsManipulator>();
            builder.RegisterType<PackageReferenceConflictChecker>().As<IPackageReferenceConflictChecker>();
        }
    }
}
