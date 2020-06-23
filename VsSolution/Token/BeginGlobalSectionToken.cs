using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    internal class BeginGlobalSectionToken : IToken
    {
        public override string ToString()
        {
            return "GlobalSection";
        }
    }
}