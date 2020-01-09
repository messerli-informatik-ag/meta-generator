using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class BeginGlobalToken : IToken
    {
        public override string ToString()
        {
            return "Global";
        }
    }
}