using System.CommandLine.Invocation;

namespace Messerli.MetaGenerator
{
    internal interface IPluginSelection
    {
        void StartPluginInteractive(InvocationContext context);

        void StartPlugin(InvocationContext context, string generatorName);
    }
}