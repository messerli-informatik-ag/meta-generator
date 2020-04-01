using Autofac;
using Messerli.FileManipulatorAbstractions;

namespace Messerli.FileManipulator
{
    public class FileManipulatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GlobalJsonManipulator>().As<IGlobalJsonManipulator>();
        }
    }
}