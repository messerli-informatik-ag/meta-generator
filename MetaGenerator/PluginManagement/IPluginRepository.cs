using System.Collections.Generic;
using System.Threading.Tasks;

namespace Messerli.MetaGenerator.PluginManagement
{
    internal interface IPluginRepository
    {
        string Name { get; }

        Task<IEnumerable<PluginPackage>> Plugins();
    }
}
