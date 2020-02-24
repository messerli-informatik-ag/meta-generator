using System;
using System.Collections.Generic;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class IntegerRequester : AbstractVariableRequester
    {
        public IntegerRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        {
            var dummy = 0;
            yield return new SimpleValidation(input => int.TryParse(input, out dummy), "Please enter true or false (no numeric input allowed).");
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please enter a valid integer for '{0}':");

            return QueryValueFromUser(variable).Match(
                none: () => throw new NotImplementedException("cannot not happen"),
                some: intValue => intValue.ToString());
        }

        private Option<int> QueryValueFromUser(IUserInputDescription variable)
        {
            return ValidatedUserInput
                .GetValidatedValue(variable, RequesterValidations(variable))
                .Match(none: () => QueryValueFromUser(variable), some: ToInteger);
        }

        private static Option<int> ToInteger(string validatedIntegerString)
        {
            return Option.Some(int.Parse(validatedIntegerString));
        }
    }
}