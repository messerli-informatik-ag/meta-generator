using System;
using System.Collections.Generic;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class DateTimeRequester : AbstractVariableRequester
    {
        public DateTimeRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override IEnumerable<IValidation> RequesterValidations()
        {
            yield break;
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please a valid date and time for '{0}':");

            throw new NotImplementedException();
        }
    }
}