using System;
using System.Collections.Generic;
using System.Linq;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class PathRequester : AbstractVariableRequester
    {
        public PathRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        {
            yield break;
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please enter a valid path for '{0}':");

            return QueryValueFromUser(variable).Match(
                none: () => throw new NotImplementedException("cannot not happen"),
                some: Functional.Identity);
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return ValidatedUserInput
                .GetValidatedValue(variable, Enumerable.Empty<IValidation>())
                .Match(none: () => QueryValueFromUser(variable), some: Option.Some);
        }
    }
}