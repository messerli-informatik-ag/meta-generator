using System;
using System.Collections.Generic;
using System.Linq;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Messerli.MetaGenerator.UserInput.Utility;

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

            return Retry(() => QueryValueFromUser(variable));
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
            => ValidatedUserInput.GetValidatedValue(variable, Enumerable.Empty<IValidation>());
    }
}
