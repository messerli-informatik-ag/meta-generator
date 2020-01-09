using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class ClosedParenthesisToken : IToken
    {
        public override string ToString()
        {
            return ")";
        }
    }
}