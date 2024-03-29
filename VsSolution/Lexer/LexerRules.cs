﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Messerli.Lexer;
using Messerli.Lexer.Rules;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Lexer;

internal class LexerRules
{
    private readonly HashSet<char> _guidCharacters = new() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', '{', '}', '-' };
    private bool _inSection;
    private bool _inContext;

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
        yield return new LexerRuleWithContext(AnythingButSeperatorOrWhiteSpace, SectionContext, ScanSectionWord, 3);
    }

    private static Lexeme ScanSectionWord(ILexerReader reader)
    {
        var startPosition = reader.Position;
        var word = new StringBuilder();
        while (reader.Peek().Match(none: false, some: AnythingButSeperator))
        {
            _ = reader.Read().AndThen(c => word.Append(c));
        }

        return new Lexeme(new WordToken(word.ToString().Trim()), new Position(startPosition, reader.Position - startPosition));
    }

    private bool SectionContext(List<Lexeme> list)
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

    private static bool AnythingButSeperator(char character)
    {
        return character != '=' && character != '\r' && character != '\n';
    }

    private static bool AnythingButSeperatorOrWhiteSpace(char character)
    {
        return AnythingButSeperator(character) && !char.IsWhiteSpace(character);
    }

    private static Lexeme ScanString(ILexerReader reader)
    {
        var startPosition = reader.Position;
        var quotedString = new StringBuilder();

        while (reader.Peek().Match(none: false, some: c => IsNotSecondQuote(c, startPosition == reader.Position)))
        {
            _ = reader.Read().AndThen(c => quotedString.Append(c));
        }

        // consume second quote
        _ = reader.Read().AndThen(c => quotedString.Append(c));

        return new Lexeme(new StringToken(quotedString.ToString()), new Position(startPosition, reader.Position - startPosition));
    }

    private static bool IsNotSecondQuote(in char character, bool isFirstCharacter)
    {
        return isFirstCharacter || character != '"';
    }

    private bool IsGuidCharacter(char character)
    {
        return _guidCharacters.Contains(character);
    }

    private static Lexeme ScanNumber(ILexerReader reader)
    {
        var startPosition = reader.Position;
        var number = new StringBuilder();

        while (reader.Peek().Match(none: false, some: char.IsDigit))
        {
            _ = reader.Read().AndThen(c => number.Append(c));
        }

        return new Lexeme(new NumberToken(int.Parse(number.ToString())), new Position(startPosition, reader.Position - startPosition));
    }

    private static Lexeme ScanWord(ILexerReader reader)
    {
        var startPosition = reader.Position;
        var word = new StringBuilder();

        while (reader.Peek().Match(none: false, some: char.IsLetter))
        {
            _ = reader.Read().AndThen(c => word.Append(c));
        }

        return new Lexeme(new WordToken(word.ToString()), new Position(startPosition, reader.Position - startPosition));
    }
}
