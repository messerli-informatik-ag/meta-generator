using System.Reflection;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface IExecutingPluginAssemblyProvider
    {
        bool HasPluginContext { get; }

        Assembly PluginAssembly { get; set; }
    }
}
