using System.Collections.Generic;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    public class BoolRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;

        public BoolRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please enter true or false for '{0}':");

            return QueryValueFromUser(variable).AndThen(boolValue => boolValue.ToString());
        }

        private Option<bool> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, GetBoolValidation())
                .Match(() => QueryValueFromUser(variable), ToBool);
        }

        private static Option<bool> ToBool(string validatedBoolString)
        {
            return Option.Some(bool.Parse(validatedBoolString));
        }

        private static IEnumerable<IValidation> GetBoolValidation()
        {
            var dummy = false;
            yield return new SimpleValidation(input => bool.TryParse(input, out dummy), "Please enter true or false (no numeric input allowed).");
        }
    }
}