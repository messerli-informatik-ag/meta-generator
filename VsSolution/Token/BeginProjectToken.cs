using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class BeginProjectToken : IToken
    {
        public override string ToString()
        {
            return "Project";
        }
    }
}