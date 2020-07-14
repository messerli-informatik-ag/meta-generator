using System;
using System.Collections.Generic;
using apophis.Lexer;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser
{
    public static class TokenWalkerExtensions
    {
        private static readonly HashSet<Type> WhiteSpaceTypes = new HashSet<Type> { typeof(SpaceToken), typeof(TabToken), typeof(NewLineToken) };
        private static readonly HashSet<Type> BareStringEndCharacters = new HashSet<Type> { typeof(AssignToken), typeof(NewLineToken) };

        public static string ConsumeWord(this TokenWalker tokenWalker)
        {
            var lexem = tokenWalker.Pop();

            if (lexem.Token is WordToken word)
            {
                return word.Word;
            }

            throw new ParseException($"Next token to be expected a Word but it was: {lexem.Token}");
        }

        public static void ConsumeWord(this TokenWalker tokenWalker, string expectedWord)
        {
            tokenWalker.ConsumeAllWhiteSpace();
            var lexem = tokenWalker.Pop();

            if (!(lexem.Token is WordToken word) || word.Word != expectedWord)
            {
                throw new ParseException($"Next token to be expected a Word({expectedWord}) but it was: {lexem.Token}");
            }
        }

        public static int ConsumeNumber(this TokenWalker tokenWalker)
        {
            tokenWalker.ConsumeAllWhiteSpace();
            var lexem = tokenWalker.Pop();

            return lexem.Token switch
            {
                NumberToken number => number.Number,
                _ => throw new ParseException($"Next token to be expected a Number but it was: {lexem.Token}"),
            };
        }

        public static string ConsumeString(this TokenWalker tokenWalker)
        {
            tokenWalker.ConsumeAllWhiteSpace();
            var lexem = tokenWalker.Pop();

            return lexem.Token switch
            {
                WordToken word => word.Word,
                StringToken quotedString => quotedString.String,
                _ => throw new ParseException($"Next token to be expected a \" or string but it was: {lexem.Token}"),
            };
        }

        public static KeyValuePair<string, string> ConsumeVariable(this TokenWalker tokenWalker)
        {
            tokenWalker.ConsumeAllWhiteSpace();
            var key = tokenWalker.ConsumeWord();
            tokenWalker.Consume<AssignToken>();
            tokenWalker.ConsumeAllWhiteSpace();
            var value = tokenWalker.ConsumeWord();
            tokenWalker.ConsumeAllWhiteSpace();

            return new KeyValuePair<string, string>(key, value);
        }

        public static Guid ConsumeGuid(this TokenWalker tokenWalker)
        {
            tokenWalker.ConsumeAllWhiteSpace();
            var lexem = tokenWalker.Pop();

            return lexem.Token switch
            {
                StringToken guid => Guid.Parse(guid.String),
                WordToken guid => Guid.Parse(guid.Word),
                _ => throw new ParseException($"Next token to be expected a \" or Guid  but it was: {lexem.Token}"),
            };
        }

        public static void ConsumeAllWhiteSpace(this TokenWalker tokenWalker)
        {
            while (WhiteSpaceTypes.Contains(tokenWalker.Peek().Token.GetType()))
            {
                tokenWalker.Pop();
            }
        }
    }
}
