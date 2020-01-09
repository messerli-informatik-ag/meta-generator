using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class BeginProjectSectionToken : IToken
    {
        public override string ToString()
        {
            return "ProjectSection";
        }
    }
}