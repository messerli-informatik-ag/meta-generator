using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class AssignToken : IToken
    {
        public override string ToString()
        {
            return "=";
        }
    }
}
