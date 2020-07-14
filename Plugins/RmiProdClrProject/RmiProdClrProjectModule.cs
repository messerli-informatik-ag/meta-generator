using Autofac;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.RmiProdClrProject
{
    public class RmiProdClrProjectModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RmiProdClrProjectGenerator>().As<IMetaGenerator>();
        }
    }
}