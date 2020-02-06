using System.Collections.Generic;

namespace Messerli.ToolLoaderAbstractions
{
    public interface ITool
    {
        public bool IsAvailable();

        public void Execute(IEnumerable<string> arguments);
    }
}