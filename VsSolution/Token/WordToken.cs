using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class WordToken : IToken
    {
        public WordToken(string word)
        {
            Word = word;
        }

        public string Word { get; }

        public override string ToString()
        {
            return Word;
        }
    }
}
