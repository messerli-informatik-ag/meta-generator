using System;
using System.Collections.Generic;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Messerli.MetaGenerator.UserInput.Utility;

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

            return Retry(() => QueryValueFromUser(variable)).ToString();
        }

        private Option<int> QueryValueFromUser(IUserInputDescription variable)
            => ValidatedUserInput
                   .GetValidatedValue(variable, RequesterValidations(variable))
                   .SelectMany(ToInteger);

        private static Option<int> ToInteger(string validatedIntegerString)
            => Option.Some(int.Parse(validatedIntegerString));
    }
}
