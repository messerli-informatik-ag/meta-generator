using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class EndGlobalSectionToken : IToken
    {
        public override string ToString()
        {
            return "EndGlobalSection";
        }
    }
}