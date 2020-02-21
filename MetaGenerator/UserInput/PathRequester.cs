using System;
using System.Collections.Generic;
using System.Linq;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class PathRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;
        private readonly IEnumerable<IValidation> _requesterValidations = Enumerable.Empty<IValidation>();

        public PathRequester(IValidatedUserInput validatedUserInput)
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
            _validatedUserInput.WriteQuestion(variable, "Please enter a valid path for '{0}':");

            return QueryValueFromUser(variable).Match(
                none: () => throw new NotImplementedException("cannot not happen"),
                some: Functional.Identity);
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, Enumerable.Empty<IValidation>())
                .Match(none: () => QueryValueFromUser(variable), some: Option.Some);
        }
    }
}