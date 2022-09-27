using System.Collections.Generic;
using Funcky.Monads;

namespace Messerli.MetaGeneratorAbstractions.UserInput;

public interface IValidatedUserInput
{
    Option<string> ValidateArgument(IUserInputDescription variable, Option<string> userArgument, IEnumerable<IValidation> requesterValidations);

    Option<string> GetValidatedValue(IUserInputDescription variable, IEnumerable<IValidation> requesterValidations);

    void WriteQuestion(IUserInputDescription variable, string defaultQuestion);
}
