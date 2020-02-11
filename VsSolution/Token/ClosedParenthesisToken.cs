using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    internal class ClosedParenthesisToken : IToken
    {
        public override string ToString()
        {
            return ")";
        }
    }
}