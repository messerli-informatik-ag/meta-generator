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
            builder.RegisterType<ProjectManipulator>().As<IProjectManipulator>();
            builder.RegisterType<MicrosoftBuildAssemblyLoader>().As<IMicrosoftBuildAssemblyLoader>();
        }
    }
}