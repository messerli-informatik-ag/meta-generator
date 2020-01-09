using System;
using System.Collections.Generic;
using System.Text;
using apophis.Lexer;
using apophis.Lexer.Rules;
using Messerli.VsSolution.Parser;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Lexer
{
    public class LexerRules : ILexerRules
    {
        private readonly HashSet<char> _guidCharacters = new HashSet<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', '{', '}', '-' };

        /// <inheritdoc />
        public IEnumerable<ILexerRule> GetRules()
        {
            yield return new LexerRule(c => c == '{', ScanGuid);
            yield return new LexerRule(char.IsDigit, ScanNumber);
            yield return new LexerRule(char.IsLetter, ScanWord);
            yield return new SimpleLexerRule<BeginGlobalToken>("Global");
            yield return new SimpleLexerRule<EndGlobalToken>("EndGlobal");
            yield return new SimpleLexerRule<BeginGlobalSectionToken>("GlobalSection");
            yield return new SimpleLexerRule<EndGlobalSectionToken>("EndGlobalSection");
            yield return new SimpleLexerRule<BeginProjectToken>("Project");
            yield return new SimpleLexerRule<EndProjectToken>("EndProject");
            yield return new SimpleLexerRule<BeginProjectSectionToken>("ProjectSection");
            yield return new SimpleLexerRule<EndProjectSectionToken>("EndProjectSection");
            yield return new SimpleLexerRule<OpenParenthesisToken>("(");
            yield return new SimpleLexerRule<ClosedParenthesisToken>(")");
            yield return new SimpleLexerRule<AssignToken>("=");
            yield return new SimpleLexerRule<HashToken>("#");
            yield return new SimpleLexerRule<CommaToken>(",");
            yield return new SimpleLexerRule<DotToken>(".");
            yield return new SimpleLexerRule<PathSeperatorToken>("\\");
            yield return new SimpleLexerRule<QuoteToken>("\"");
            yield return new SimpleLexerRule<PipeToken>("|");
            yield return new SimpleLexerRule<NewLineToken>("\r\n");
            yield return new SimpleLexerRule<NewLineToken>("\n");
            yield return new SimpleLexerRule<NewLineToken>("\r");
            yield return new SimpleLexerRule<SpaceToken>(" ");
            yield return new SimpleLexerRule<TabToken>("\t");
        }

        private Lexem ScanGuid(ILexerReader reader)
        {
            var startPosition = reader.Position;
            var guid = new StringBuilder();
            while (reader.Peek().Match(false, IsGuidCharacter))
            {
                reader.Read().AndThen(c => guid.Append(c));
            }

            return new Lexem(new GuidToken(Guid.Parse(guid.ToString())), new Position(startPosition, reader.Position - startPosition));
        }

        private bool IsGuidCharacter(char character)
        {
            return _guidCharacters.Contains(character);
        }

        private Lexem ScanNumber(ILexerReader reader)
        {
            var startPosition = reader.Position;
            var number = new StringBuilder();
            while (reader.Peek().Match(false, char.IsDigit))
            {
                reader.Read().AndThen(c => number.Append(c));
            }

            return new Lexem(new NumberToken(int.Parse(number.ToString())), new Position(startPosition, reader.Position - startPosition));
        }

        private Lexem ScanWord(ILexerReader reader)
        {
            var startPosition = reader.Position;
            var word = new StringBuilder();
            while (reader.Peek().Match(false, char.IsLetter))
            {
                reader.Read().AndThen(c => word.Append(c));
            }

            return new Lexem(new WordToken(word.ToString()), new Position(startPosition, reader.Position - startPosition));
        }
    }
}
