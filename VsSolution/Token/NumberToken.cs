using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    internal class NumberToken : IToken
    {
        public NumberToken(int number)
        {
            Number = number;
        }

        public int Number { get; }

        public override string ToString()
        {
            return $"{Number}";
        }
    }
}