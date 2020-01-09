using System;
using System.Collections.Generic;
using System.Text;
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
                _ => throw new ParseException($"Next token to be expected a Number but it was: {lexem.Token}")
            };
        }

        public static string ConsumeString(this TokenWalker tokenWalker)
        {
            tokenWalker.ConsumeAllWhiteSpace();
            var lexem = tokenWalker.Pop();

            return lexem.Token switch
            {
                WordToken word => word.Word,
                QuoteToken _ => QuotedString(tokenWalker),
                _ => throw new ParseException($"Next token to be expected a \" or string but it was: {lexem.Token}")
            };
        }

        public static KeyValuePair<string, string> ConsumeBareVariable(this TokenWalker tokenWalker)
        {
            var key = tokenWalker.ConsumeBareString();
            tokenWalker.Consume<AssignToken>();
            var value = tokenWalker.ConsumeBareString();
            tokenWalker.Consume<NewLineToken>();

            return new KeyValuePair<string, string>(key, value);
        }

        public static string ConsumeBareString(this TokenWalker tokenWalker)
        {
            tokenWalker.ConsumeAllWhiteSpace();

            var result = new StringBuilder();
            while (BareStringEndCharacters.Contains(tokenWalker.Peek().Token.GetType()) == false)
            {
                result.Append(tokenWalker.Pop().Token);
            }

            return result.ToString().TrimEnd();
        }

        public static Guid ConsumeGuid(this TokenWalker tokenWalker)
        {
            tokenWalker.ConsumeAllWhiteSpace();
            var lexem = tokenWalker.Pop();

            return lexem.Token switch
            {
                GuidToken guid => guid.Guid,
                QuoteToken _ => QuotedGuid(tokenWalker),
                _ => throw new ParseException($"Next token to be expected a \" or Guid  but it was: {lexem.Token}")
            };
        }

        public static void ConsumeAllWhiteSpace(this TokenWalker tokenWalker)
        {
            while (WhiteSpaceTypes.Contains(tokenWalker.Peek().Token.GetType()))
            {
                tokenWalker.Pop();
            }
        }

        private static string QuotedString(TokenWalker tokenWalker)
        {
            var result = new StringBuilder();

            while (!tokenWalker.NextIs<QuoteToken>())
            {
                result.Append(tokenWalker.Pop().Token);
            }

            tokenWalker.Consume<QuoteToken>();

            return result.ToString();
        }

        private static Guid QuotedGuid(TokenWalker tokenWalker)
        {
            var result = tokenWalker.ConsumeGuid();

            tokenWalker.Consume<QuoteToken>();

            return result;
        }
    }
}
