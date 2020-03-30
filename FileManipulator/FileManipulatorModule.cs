using Autofac;

namespace Messerli.FileManipulator
{
    public class FileManipulatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GlobalJsonLoader>().As<IGlobalJsonLoader>();
        }
    }
}