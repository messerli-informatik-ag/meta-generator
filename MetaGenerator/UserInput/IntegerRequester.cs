using System;
using System.Collections.Generic;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class IntegerRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;
        private readonly IEnumerable<IValidation> _requesterValidations = GetIntegerValidation();

        public IntegerRequester(IValidatedUserInput validatedUserInput)
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
            _validatedUserInput.WriteQuestion(variable, "Please enter a valid integer for '{0}':");

            return QueryValueFromUser(variable).Match(
                none: () => throw new NotImplementedException("cannot not happen"),
                some: intValue => intValue.ToString());
        }

        private Option<int> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, _requesterValidations)
                .Match(none: () => QueryValueFromUser(variable), some: ToInteger);
        }

        private static Option<int> ToInteger(string validatedIntegerString)
        {
            return Option.Some(int.Parse(validatedIntegerString));
        }

        private static IEnumerable<IValidation> GetIntegerValidation()
        {
            var dummy = 0;
            yield return new SimpleValidation(input => int.TryParse(input, out dummy), "Please enter true or false (no numeric input allowed).");
        }
    }
}