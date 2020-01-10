using apophis.Lexer;
using Messerli.VsSolution.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Parser.GlobalSection;
using Messerli.VsSolution.Parser.ProjectSection;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser
{
    internal class SolutionParser
    {
        private readonly TokenWalker _tokenWalker;
        private readonly HeaderParser _headerParser;
        private readonly ProjectParser _projectParser;
        private readonly GlobalSectionParser _globalSectionParser;
        private readonly Solution _solution = new Solution();

        public SolutionParser(TokenWalker tokenWalker, HeaderParser headerParser, ProjectParser projectParser, GlobalSectionParser globalSectionParser)
        {
            _tokenWalker = tokenWalker;
            _headerParser = headerParser;
            _projectParser = projectParser;
            _globalSectionParser = globalSectionParser;
        }

        public static SolutionParser Create()
        {
            // Create the object tree without DI Framework
            var lexerRules = new LexerRules();
            var tokenizer = new Tokenizer(lexerRules, s => new LexerReader(s));
            var tokenWalker = new TokenWalker(tokenizer, () => new EpsilonToken());
            var variableParser = new VariableParser();
            var headerParser = new HeaderParser(variableParser);
            var projectParser = new ProjectParser();
            var globalSectionParser = new GlobalSectionParser();
            return new SolutionParser(tokenWalker, headerParser, projectParser, globalSectionParser);
        }

        public Solution Parse(string solutionContent)
        {
            _tokenWalker.Scan(solutionContent);

            Parse(_tokenWalker, _solution);

            return _solution;
        }

        private void Parse(TokenWalker tokenWalker, Solution solution)
        {
            _headerParser.Parse(tokenWalker, solution);
            ParseProjects(tokenWalker, solution);
            ParseGlobalSections(tokenWalker, solution);

            // any whitespace left...
            tokenWalker.ConsumeAllWhiteSpace();

            // Verify that we are at the end of the file
            tokenWalker.Consume<EpsilonToken>();
        }

        private void ParseProjects(TokenWalker tokenWalker, Solution solution)
        {
            while (tokenWalker.NextIs<BeginProjectToken>())
            {
                _projectParser.Parse(tokenWalker, solution);
            }
        }

        private void ParseGlobalSections(TokenWalker tokenWalker, Solution solution)
        {
            tokenWalker.Consume<BeginGlobalToken>();

            tokenWalker.ConsumeAllWhiteSpace();
            while (tokenWalker.NextIs<BeginGlobalSectionToken>())
            {
                _globalSectionParser.Parse(tokenWalker, solution);
            }

            tokenWalker.Consume<EndGlobalToken>();
        }
    }
}