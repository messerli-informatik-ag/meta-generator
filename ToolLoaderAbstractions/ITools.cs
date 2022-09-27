using System.Collections.Generic;
using Funcky.Monads;

namespace Messerli.ToolLoaderAbstractions;

public interface ITools
{
    void RegisterTool(string name, string executable, Option<string> specificPath = default);

    IEnumerable<KeyValuePair<string, ITool>> VerifyTools();

    ITool GetTool(string name);

    ITool CreateToolFromPath(string path);
}
