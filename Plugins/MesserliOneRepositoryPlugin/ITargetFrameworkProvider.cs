using System.Collections.Generic;
using Messerli.MetaGeneratorAbstractions.Json;

namespace Messerli.MesserliOneRepositoryPlugin
{
    public interface ITargetFrameworkProvider
    {
        IEnumerable<SelectionValue> GetSelection();
    }
}