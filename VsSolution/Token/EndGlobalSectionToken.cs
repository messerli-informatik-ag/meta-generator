using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    internal class EndGlobalSectionToken : IToken
    {
        public override string ToString()
        {
            return "EndGlobalSection";
        }
    }
}