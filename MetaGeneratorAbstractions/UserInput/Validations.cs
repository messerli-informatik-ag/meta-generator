using System.Linq;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public static class Validations
    {
        public static IValidation ValuePresent => SimpleValidation.Create(value => value.Length > 0, "Value '{0}' must not be empty.");

        public static IValidation NoWhiteSpace => SimpleValidation.Create(value => value.Any(char.IsWhiteSpace) == false, "Value '{0}' must not contain whitespace");
    }
}
