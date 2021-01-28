using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Messerli.MetaGenerator.PluginManagement
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginDllPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginDllPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            try
            {
                // We try to load common libraries from the Default loading context (avoid multiple loads of the same library i.e. Autofac)
                return Default.LoadFromAssemblyName(assemblyName);
            }
            catch (Exception)
            {
                var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

                return assemblyPath is null
                    ? null
                    : LoadFromAssemblyPath(assemblyPath);
            }
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return libraryPath is null
                ? IntPtr.Zero
                : LoadUnmanagedDllFromPath(libraryPath);
        }
    }
}
