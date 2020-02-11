using System.Collections.Generic;

namespace Messerli.ToolLoaderAbstractions
{
    public interface ITool
    {
        void Execute(IEnumerable<string> arguments, string workingDirectory);

        bool IsAvailable();
    }
}