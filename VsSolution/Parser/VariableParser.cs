using System;
using Messerli.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser;

internal class VariableParser
{
    public VersionVariable ParseVersion(TokenWalker tokenWalker)
    {
        var variableName = tokenWalker.ConsumeWord();

        tokenWalker.ConsumeAllWhiteSpace();
        tokenWalker.Consume<AssignToken>();

        var major = tokenWalker.ConsumeNumber();
        tokenWalker.Consume<DotToken>();
        var minor = tokenWalker.ConsumeNumber();
        tokenWalker.Consume<DotToken>();
        var build = tokenWalker.ConsumeNumber();
        tokenWalker.Consume<DotToken>();
        var revision = tokenWalker.ConsumeNumber();
        tokenWalker.Consume<NewLineToken>();

        return new VersionVariable(variableName, new Version(major, minor, build, revision));
    }
}
