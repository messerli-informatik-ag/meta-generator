using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    internal class PathSeperatorToken : IToken
    {
        public override string ToString()
        {
            return "\\";
        }
    }
}