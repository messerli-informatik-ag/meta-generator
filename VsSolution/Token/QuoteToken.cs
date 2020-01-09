using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class QuoteToken : IToken
    {
        public override string ToString()
        {
            return "\"";
        }
    }
}
