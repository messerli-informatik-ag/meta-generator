using System;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public interface IValidation
    {
        Func<string, bool> Validation { get; }

        string Message { get; }
    }
}
