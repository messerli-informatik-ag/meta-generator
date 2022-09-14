using System.Collections.Generic;
using Messerli.MetaGeneratorAbstractions.Json;

namespace Messerli.NativeProjectsPlugin;

internal interface ITfsPaths
{
    IEnumerable<SelectionValue> Branches();

    string BranchRootPath();
}
