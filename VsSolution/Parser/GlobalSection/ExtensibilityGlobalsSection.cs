using System.Text;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.GlobalSection
{
    internal class ExtensibilityGlobalsSection : IGlobalSection
    {
        private const string SolutionGuidKey = "SolutionGuid";

        public void Parse(TokenWalker tokenWalker, Solution solution)
        {
            while (tokenWalker.NextIs<EndGlobalSectionToken>() == false)
            {
                tokenWalker.ConsumeAllWhiteSpace();
                tokenWalker.ConsumeWord(SolutionGuidKey);
                tokenWalker.ConsumeAllWhiteSpace();
                tokenWalker.Consume<AssignToken>();
                tokenWalker.ConsumeAllWhiteSpace();
                solution.Guid = tokenWalker.ConsumeGuid();
                tokenWalker.ConsumeAllWhiteSpace();
            }
        }

        public void Serialize(Solution solution, StringBuilder result)
        {
            result.AppendLine($"\t\t{SolutionGuidKey} = {solution.Guid.SolutionFormat()}");
        }

        public bool Exists(Solution solution)
        {
            return true;
        }
    }
}