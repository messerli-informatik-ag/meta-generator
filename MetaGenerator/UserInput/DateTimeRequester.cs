using System;
using System.Collections.Generic;
using System.Linq;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class DateTimeRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;
        private readonly IEnumerable<IValidation> _requesterValidations = Enumerable.Empty<IValidation>();

        public DateTimeRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public string RequestValue(IUserInputDescription variable, Option<string> userArgument)
        {
            return _validatedUserInput.ValidateArgument(variable, userArgument, _requesterValidations)
                .Match(() => InteractiveQuery(variable), Functional.Identity);
        }

        private string InteractiveQuery(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please a valid date and time for '{0}':");

            throw new NotImplementedException();
        }
    }
}