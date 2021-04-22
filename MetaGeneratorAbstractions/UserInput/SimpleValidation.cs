using System;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public class SimpleValidation : IValidation
    {
        private SimpleValidation(Func<string, bool> validation, string message)
        {
            Validation = validation;
            Message = message;
        }

        public Func<string, bool> Validation { get; }

        public string Message { get; }

        public static IValidation Create(Func<string, bool> isValidInput, string message)
            => new SimpleValidation(isValidInput, message);
    }
}
