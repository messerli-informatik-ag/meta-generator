using Messerli.Lexer.Tokens;

namespace Messerli.VsSolution.Token;

internal class BeginGlobalToken : IToken
{
    public override string ToString()
    {
        return "Global";
    }
}
