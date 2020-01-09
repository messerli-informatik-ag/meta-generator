using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class BeginGlobalSectionToken : IToken
    {
        public override string ToString()
        {
            return "GlobalSection";
        }
    }
}