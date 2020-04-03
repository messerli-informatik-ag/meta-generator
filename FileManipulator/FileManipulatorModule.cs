using Autofac;
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
        }
    }
}
