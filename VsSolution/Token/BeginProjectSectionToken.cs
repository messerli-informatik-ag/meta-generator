using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    internal class BeginProjectSectionToken : IToken
    {
        public override string ToString()
        {
            return "ProjectSection";
        }
    }
}