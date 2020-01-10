using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class StringToken : IToken
    {
        public StringToken(string quotedString)
        {
            String = quotedString.Trim('"');
        }

        public string String { get; }

        public override string ToString()
        {
            return String;
        }
    }
}
