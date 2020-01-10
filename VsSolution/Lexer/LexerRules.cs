using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using apophis.Lexer;
using apophis.Lexer.Rules;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Lexer
{
    public class LexerRules : ILexerRules
    {
        private readonly HashSet<char> _guidCharacters = new HashSet<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', '{', '}', '-' };
        private bool _inSection;
        private bool _inContext;

        /// <inheritdoc />
        public IEnumerable<ILexerRule> GetRules()
        {
            yield return new LexerRule(c => c == '"', ScanString);
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
            yield return new SimpleLexerRule<PipeToken>("|");
            yield return new SimpleLexerRule<NewLineToken>("\r\n");
            yield return new SimpleLexerRule<NewLineToken>("\n");
            yield return new SimpleLexerRule<NewLineToken>("\r");
            yield return new SimpleLexerRule<SpaceToken>(" ");
            yield return new SimpleLexerRule<TabToken>("\t");
            yield return new ContextedLexerRule(AnythingButSeperatorOrWhiteSpace, SectionContext, ScanSectionWord, 3);
        }

        private Lexem ScanSectionWord(ILexerReader reader)
        {
            var startPosition = reader.Position;
            var word = new StringBuilder();
            while (reader.Peek().Match(false, AnythingButSeperator))
            {
                reader.Read().AndThen(c => word.Append(c));
            }

            return new Lexem(new WordToken(word.ToString().Trim()), new Position(startPosition, reader.Position - startPosition));
        }

        private bool SectionContext(List<Lexem> list)
        {
            if (list.Any())
            {
                var lastLexem = list.Last();

                if (lastLexem.Token is BeginGlobalSectionToken || lastLexem.Token is BeginProjectSectionToken)
                {
                    _inSection = true;
                }

                if (_inSection && lastLexem.Token is WordToken word && LoadingOrders.IsValidLoadingOrder(word.Word))
                {
                    _inContext = true;
                }

                if (lastLexem.Token is EndGlobalSectionToken || lastLexem.Token is EndProjectSectionToken)
                {
                    _inSection = false;
                    _inContext = false;
                }
            }

            return _inContext;
        }

        private bool AnythingButSeperator(char character)
        {
            return character != '=' && character != '\r' && character != '\n';
        }

        private bool AnythingButSeperatorOrWhiteSpace(char character)
        {
            return AnythingButSeperator(character) && !char.IsWhiteSpace(character);
        }

        private Lexem ScanString(ILexerReader reader)
        {
            var startPosition = reader.Position;
            var quotedString = new StringBuilder();

            while (reader.Peek().Match(false, c => IsNotSecondQuote(c, startPosition == reader.Position)))
            {
                reader.Read().AndThen(c => quotedString.Append(c));
            }

            // consume second quote
            reader.Read().AndThen(c => quotedString.Append(c));

            return new Lexem(new StringToken(quotedString.ToString()), new Position(startPosition, reader.Position - startPosition));
        }

        private bool IsNotSecondQuote(in char character, bool isFirstCharacter)
        {
            return isFirstCharacter || character != '"';
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
