using System;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public class SimpleValidation : IValidation
    {
        private SimpleValidation(Predicate<string> validation, string message)
        {
            Validation = validation;
            Message = message;
        }

        public Predicate<string> Validation { get; }

        public string Message { get; }

        public static IValidation Create(Predicate<string> isValidInput, string message)
            => new SimpleValidation(isValidInput, message);
    }
}
