using System;
using System.Collections.Generic;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class StringRequester : AbstractVariableRequester
    {
        public StringRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override IEnumerable<IValidation> RequesterValidations()
        {
            yield break;
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please enter a value for '{0}':");

            return QueryValueFromUser(variable).Match(
                none: () => throw new NotImplementedException("cannot not happen"),
                some: Functional.Identity);
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return ValidatedUserInput
                .GetValidatedValue(variable, RequesterValidations())
                .Match(none: () => QueryValueFromUser(variable), some: Option.Some);
        }
    }
}