using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    internal class NewLineToken : IToken, ILineBreakToken
    {
        public override string ToString()
        {
            return "\r\n";
        }
    }
}
