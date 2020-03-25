using System.Collections.Generic;
using System.Threading.Tasks;

namespace Messerli.BackbonePluginTemplatePlugin.Variants
{
    public interface IPluginVariant
    {
        List<Task> CreateTemplateFiles();
    }
}
