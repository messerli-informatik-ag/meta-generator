using System;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser
{
    internal class HeaderParser
    {
        private readonly VariableParser _variableParser;

        public HeaderParser(VariableParser variableParser)
        {
            _variableParser = variableParser;
        }

        public void Parse(TokenWalker tokenWalker, Solution solution)
        {
            ParseFormatVersion(tokenWalker, solution);
            ParseVersionComment(tokenWalker);

            solution.VisualStudioVersion = ParseVariable(tokenWalker, "VisualStudioVersion");
            solution.MinimumVisualStudioVersion = ParseVariable(tokenWalker, "MinimumVisualStudioVersion");
        }

        private Version ParseVariable(TokenWalker tokenWalker, string variableName)
        {
            var variable = _variableParser.ParseVersion(tokenWalker);

            if (variable.Name != variableName)
            {
                throw new ParseException($"Expected Variable {variableName} but got {variable.Name}");
            }

            return variable.Value;
        }

        private static void ParseVersionComment(TokenWalker tokenWalker)
        {
            // we treat this as a comment and ignore anything in this line.
            tokenWalker.Consume<HashToken>();

            while (!tokenWalker.NextIs<NewLineToken>())
            {
                tokenWalker.Pop();
            }

            tokenWalker.Consume<NewLineToken>();
        }

        private static void ParseFormatVersion(TokenWalker tokenWalker, Solution solution)
        {
            tokenWalker.ConsumeWord("Microsoft");
            tokenWalker.ConsumeWord("Visual");
            tokenWalker.ConsumeWord("Studio");
            tokenWalker.ConsumeWord("Solution");
            tokenWalker.ConsumeWord("File");

            tokenWalker.Consume<CommaToken>();

            tokenWalker.ConsumeWord("Format");
            tokenWalker.ConsumeWord("Version");

            solution.FormatVersion = tokenWalker.ConsumeNumber();
            tokenWalker.Consume<DotToken>();

            tokenWalker.ConsumeNumber(); // should be 00
            tokenWalker.Consume<NewLineToken>();
        }
    }
}
