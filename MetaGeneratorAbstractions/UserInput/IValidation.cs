using System;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public interface IValidation
    {
        Predicate<string> Validation { get; }

        string Message { get; }
    }
}
