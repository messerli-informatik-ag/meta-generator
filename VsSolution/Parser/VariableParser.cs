using System;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser
{
    public class VariableParser
    {
        public VersionVariable ParseVersion(TokenWalker walker)
        {
            var variableName = walker.ConsumeWord();

            walker.ConsumeAllWhiteSpace();
            walker.Consume<AssignToken>();

            var major = walker.ConsumeNumber();
            walker.Consume<DotToken>();
            var minor = walker.ConsumeNumber();
            walker.Consume<DotToken>();
            var build = walker.ConsumeNumber();
            walker.Consume<DotToken>();
            var revision = walker.ConsumeNumber();
            walker.Consume<NewLineToken>();

            return new VersionVariable(variableName, new Version(major, minor, build, revision));
        }
    }
}