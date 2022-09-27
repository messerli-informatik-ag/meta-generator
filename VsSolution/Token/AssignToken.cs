using Messerli.Lexer.Tokens;

namespace Messerli.VsSolution.Token;

internal class AssignToken : IToken
{
    public override string ToString()
    {
        return "=";
    }
}
