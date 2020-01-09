using System;
using apophis.Lexer.Tokens;

namespace Messerli.VsSolution.Token
{
    public class GuidToken : IToken
    {
        public GuidToken(Guid guid)
        {
            Guid = guid;
        }

        public Guid Guid { get; }

        public override string ToString()
        {
            return $"{{{Guid.ToString().ToUpper()}}}";
        }
    }
}
