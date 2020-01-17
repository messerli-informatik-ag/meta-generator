using System.Collections.Generic;
using Funcky.Monads;

namespace Messerli.ProjectAbstractions.UserInput
{
    public interface IValidatedUserInput
    {
        Option<string> GetValidatedValue(IUserInputDescription variable, IEnumerable<IValidation> requesterValidations);

        void WriteQuestion(IUserInputDescription variable, string defaultQuestion);
    }
}
