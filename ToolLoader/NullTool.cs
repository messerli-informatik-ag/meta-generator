using System;
using System.Collections.Generic;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.ToolLoader;

internal class NullTool : ITool
{
    private NullTool()
    {
    }

    public string StandardOutput => string.Empty;

    public static ITool Create()
    {
        return new NullTool();
    }

    public void Execute(IEnumerable<string> arguments, string workingDirectory)
    {
        throw new NotImplementedException();
    }

    public bool IsAvailable()
    {
        return false;
    }
}
