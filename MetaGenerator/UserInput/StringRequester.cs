using System;
using System.Collections.Generic;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Messerli.MetaGenerator.UserInput.Utility;

namespace Messerli.MetaGenerator.UserInput
{
    internal class StringRequester : AbstractVariableRequester
    {
        public StringRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        {
            yield break;
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please enter a value for '{0}':");

            return Retry(() => QueryValueFromUser(variable));
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
            => ValidatedUserInput.GetValidatedValue(variable, RequesterValidations(variable));
    }
}
