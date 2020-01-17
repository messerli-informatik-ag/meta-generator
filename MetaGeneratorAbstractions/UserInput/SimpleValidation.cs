using System;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public class SimpleValidation : IValidation
    {
        public SimpleValidation(Predicate<string> validation, string message)
        {
            Validation = validation;
            Message = message;
        }

        public Predicate<string> Validation { get; }

        public string Message { get; }
    }
}
