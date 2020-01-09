using System;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser
{
    public class HeaderParser
    {
        private readonly VariableParser _variableParser;

        public HeaderParser(VariableParser variableParser)
        {
            _variableParser = variableParser;
        }

        public void Parse(TokenWalker walker, Solution solution)
        {
            ParseFormatVersion(walker, solution);
            ParseVersionComment(walker);

            solution.VisualStudioVersion = ParseVariable(walker, "VisualStudioVersion");
            solution.MinimumVisualStudioVersion = ParseVariable(walker, "MinimumVisualStudioVersion");
        }

        private Version ParseVariable(TokenWalker walker, string variableName)
        {
            var variable = _variableParser.ParseVersion(walker);

            if (variable.Name != variableName)
            {
                throw new ParseException($"Expected Variable {variableName} but got {variable.Name}");
            }

            return variable.Value;
        }

        private static void ParseVersionComment(TokenWalker walker)
        {
            walker.Consume<HashToken>();

            walker.ConsumeWord("Visual");
            walker.ConsumeWord("Studio");
            walker.ConsumeWord("Version");

            walker.ConsumeNumber(); // should be Major Version
            walker.Consume<NewLineToken>();
        }

        private static void ParseFormatVersion(TokenWalker walker, Solution solution)
        {
            walker.ConsumeWord("Microsoft");
            walker.ConsumeWord("Visual");
            walker.ConsumeWord("Studio");
            walker.ConsumeWord("Solution");
            walker.ConsumeWord("File");

            walker.Consume<CommaToken>();

            walker.ConsumeWord("Format");
            walker.ConsumeWord("Version");

            solution.FormatVersion = walker.ConsumeNumber();
            walker.Consume<DotToken>();

            walker.ConsumeNumber(); // should be 00
            walker.Consume<NewLineToken>();
        }
    }
}
