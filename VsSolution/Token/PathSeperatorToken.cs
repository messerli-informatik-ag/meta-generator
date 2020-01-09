using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class PathSeperatorToken : IToken
    {
        public override string ToString()
        {
            return "\\";
        }
    }
}