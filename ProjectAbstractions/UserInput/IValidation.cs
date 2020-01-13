using System;

namespace Messerli.ProjectAbstractions.UserInput
{
    public interface IValidation
    {
        Predicate<string> Validation { get; }

        string Message { get; }
    }
}
