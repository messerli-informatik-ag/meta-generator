using System;
using System.Reflection;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator
{
    public class ExecutingPluginAssemblyProvider : IExecutingPluginAssemblyProvider
    {
        private Assembly? _pluginAssembly;

        Assembly IExecutingPluginAssemblyProvider.PluginAssembly
        {
            get => _pluginAssembly ?? throw new Exception("No plugin has been set.");
            set
            {
                if (_pluginAssembly != null)
                {
                    throw new Exception("Plugin assembly has already been set.");
                }

                _pluginAssembly = value;
            }
        }
    }
}