using System.Reflection;

namespace Messerli.ProjectAbstractions
{
    public interface IExecutingPluginAssemblyProvider
    {
        Assembly PluginAssembly { get; set; }
    }
}
