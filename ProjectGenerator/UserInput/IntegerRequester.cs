using System.Collections.Generic;
using Funcky.Monads;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class IntegerRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;

        public IntegerRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please enter a valid integer for '{0}':");

            return QueryValueFromUser(variable).AndThen(intValue => intValue.ToString());
        }

        private Option<int> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, GetIntegerValidation())
                .Match(() => QueryValueFromUser(variable), ToInteger);
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