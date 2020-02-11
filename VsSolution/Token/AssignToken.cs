using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    internal class AssignToken : IToken
    {
        public override string ToString()
        {
            return "=";
        }
    }
}
