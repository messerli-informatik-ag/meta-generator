using System.Reflection;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface IExecutingPluginAssemblyProvider
    {
        Assembly PluginAssembly { get; set; }
    }
}
