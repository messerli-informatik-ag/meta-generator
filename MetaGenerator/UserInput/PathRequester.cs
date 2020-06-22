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

            return QueryValueFromUser(variable).GetOrElse(
                () => throw new NotImplementedException("cannot not happen"));
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return ValidatedUserInput
                .GetValidatedValue(variable, Enumerable.Empty<IValidation>())
                .OrElse(() => QueryValueFromUser(variable));
        }
    }
}
