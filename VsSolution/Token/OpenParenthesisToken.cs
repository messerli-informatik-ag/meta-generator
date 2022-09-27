using Messerli.Lexer.Tokens;

namespace Messerli.VsSolution.Token;

internal class OpenParenthesisToken : IToken
{
    public override string ToString()
    {
        return "(";
    }
}
