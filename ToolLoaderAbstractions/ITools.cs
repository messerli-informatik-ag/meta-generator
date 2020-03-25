using System.Collections.Generic;

namespace Messerli.ToolLoaderAbstractions
{
    public interface ITools
    {
        void RegisterTool(string name, string executable, string? specificPath = null);

        IEnumerable<KeyValuePair<string, ITool>> VerifyTools();

        public ITool GetTool(string name);

        ITool CreateToolFromPath(string path);
    }
}
