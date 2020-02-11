using System;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.GlobalSection
{
    internal class GlobalSectionParser
    {
        public void Parse(TokenWalker tokenWalker, Solution solution)
        {
            tokenWalker.Consume<BeginGlobalSectionToken>();
            tokenWalker.Consume<OpenParenthesisToken>();
            var sectionType = Enum.Parse<GlobalSectionType>(tokenWalker.ConsumeWord());
            tokenWalker.Consume<ClosedParenthesisToken>();

            tokenWalker.ConsumeAllWhiteSpace();
            tokenWalker.Consume<AssignToken>();

            tokenWalker.ConsumeAllWhiteSpace();
            CheckLoadingOrder(tokenWalker.ConsumeWord());

            ParseGlobalSection(sectionType, tokenWalker, solution);

            tokenWalker.Consume<EndGlobalSectionToken>();
            tokenWalker.ConsumeAllWhiteSpace();
        }

        private void CheckLoadingOrder(string loadingOrder)
        {
            if (LoadingOrders.IsValidSolutionLoadingOrder(loadingOrder) == false)
            {
                throw new ParseException($"Unknown loading Order '{loadingOrder}'");
            }
        }

        private void ParseGlobalSection(GlobalSectionType sectionType, TokenWalker tokenWalker, Solution solution)
        {
            var globalSection = GlobalSectionTypeFactory.Create(sectionType);

            globalSection.Parse(tokenWalker, solution);
        }
    }
}