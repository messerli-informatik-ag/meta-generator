using System.Linq;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public static class Validations
    {
        public static IValidation ValuePresent => new SimpleValidation(value => value.Length > 0, "Value '{0}' must not be empty.");

        public static IValidation NoWhiteSpace => new SimpleValidation(value => value.Any(char.IsWhiteSpace) == false, "Value '{0}' must not contain whitespace");
    }
}
