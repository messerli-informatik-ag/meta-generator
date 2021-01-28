using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace Messerli.MetaGenerator.PluginManagement
{
    internal interface IPluginSelection
    {
        Task<int> StartPluginInteractive(InvocationContext context);

        int StartPlugin(InvocationContext context, string generatorName);
    }
}
