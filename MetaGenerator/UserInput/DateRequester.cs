using System;
using System.Collections.Generic;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class DateRequester : AbstractVariableRequester
    {
        public DateRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        {
            yield break;
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please a valid date for '{0}':");

            throw new NotImplementedException();
        }
    }
}
