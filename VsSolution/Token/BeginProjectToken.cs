using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    internal class BeginProjectToken : IToken
    {
        public override string ToString()
        {
            return "Project";
        }
    }
}