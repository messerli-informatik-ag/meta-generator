using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class OpenParenthesisToken : IToken
    {
        public override string ToString()
        {
            return "(";
        }
    }
}