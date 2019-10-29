using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Messerli.ProjectGenerator.Test
{
    public class TestCase
    {
        private List<RomanDigit> _romanDigits = new List<RomanDigit>
        {
            new RomanDigit("M", 1000),
            new RomanDigit("CM", 900),
            new RomanDigit("D", 500),
            new RomanDigit("CD", 400),
            new RomanDigit("C", 100),
            new RomanDigit("XC", 90),
            new RomanDigit("L", 50),
            new RomanDigit("XL", 40),
            new RomanDigit("X", 10),
            new RomanDigit("IX", 9),
            new RomanDigit("V", 5),
            new RomanDigit("IV", 4),
            new RomanDigit("I", 1),
        };

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static TheoryData<string, int> GetDecimalAndRomanNumerals()
        {
            return new TheoryData<string, int>
            {
                { "I", 1 },
                { "II", 2 },
                { "III", 3 },
                { "IV", 4 },
                { "V", 5 },
                { "VI", 6 },
                { "IX", 9 },
                { "X", 10 },
                { "XX", 20 },
                { "XXX", 30 },
                { "XL", 40 },
                { "L", 50 },
                { "C", 100 },
                { "CC", 200 },
                { "D", 500 },
                { "M", 1000 },
                { "MCMXCIX", 1999 },
                { "MM", 2000 },
                { "MMI", 2001 },
            };
        }

        [Theory]
        [MemberData(nameof(GetDecimalAndRomanNumerals))]
        public void RomanNumerals(string romanResult, int number)
        {
            Assert.Equal(romanResult, ToRoman(number));
        }

        private string ToRoman(int number)
        {
            Assert.True(number >= 0);

            return number switch
                {
                0 => string.Empty,
                _ => SelectDigit(number)
            };
        }

        private string SelectDigit(int number)
        {
            var digit = _romanDigits.First(r => r.Value <= number);

            return digit.Digit + ToRoman(number - digit.Value);
        }
    }
}

public class RomanDigit
{
public RomanDigit(string digit, int value)
{
    Digit = digit;
    Value = value;
}

public string Digit { get; }

public int Value { get; }
}