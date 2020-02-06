using System.Collections.Generic;

namespace Messerli.ToolLoaderAbstractions
{
    public interface ITools
    {
        void RegisterTool(string name, string executable, string? specificPath = null);

        IEnumerable<string> VerifyTools();

        public ITool GetTool(string name);
    }
}
