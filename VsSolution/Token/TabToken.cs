using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    internal class TabToken : IToken
    {
        public override string ToString()
        {
            return "\t";
        }
    }
}