using Messerli.Lexer.Tokens;

namespace Messerli.VsSolution.Token;

internal class SpaceToken : IToken
{
    public override string ToString()
    {
        return " ";
    }
}
