using System.Collections.Generic;

namespace Messerli.ToolLoaderAbstractions
{
    public interface ITool
    {
        string StandardOutput { get; }

        void Execute(IEnumerable<string> arguments, string workingDirectory);

        bool IsAvailable();
    }
}