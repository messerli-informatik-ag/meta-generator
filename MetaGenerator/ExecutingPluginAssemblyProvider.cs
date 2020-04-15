using System;
using System.Reflection;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator
{
    internal class ExecutingPluginAssemblyProvider : IExecutingPluginAssemblyProvider
    {
        private Assembly? _pluginAssembly;

        public bool HasPluginContext => _pluginAssembly is { };

        Assembly IExecutingPluginAssemblyProvider.PluginAssembly
        {
            get => _pluginAssembly ?? throw new Exception("No plugin has been set.");
            set => _pluginAssembly = value;
        }
    }
}
