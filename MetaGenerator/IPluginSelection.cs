using System.CommandLine.Invocation;

namespace Messerli.MetaGenerator
{
    internal interface IPluginSelection
    {
        int StartPluginInteractive(InvocationContext context);

        int StartPlugin(InvocationContext context, string generatorName);
    }
}