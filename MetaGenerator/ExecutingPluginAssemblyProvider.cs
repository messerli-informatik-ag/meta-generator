using System;
using System.Reflection;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator
{
    internal class ExecutingPluginAssemblyProvider : IExecutingPluginAssemblyProvider
    {
        private Assembly? _pluginAssembly;

        Assembly IExecutingPluginAssemblyProvider.PluginAssembly
        {
            get => _pluginAssembly ?? throw new Exception("No plugin has been set.");
            set => _pluginAssembly = value;
        }

        public void Clear()
        {
            _pluginAssembly = null;
        }
    }
}