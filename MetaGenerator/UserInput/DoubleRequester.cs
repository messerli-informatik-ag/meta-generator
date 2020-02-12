using System.Collections.Generic;
using System.Globalization;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class DoubleRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;

        public DoubleRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please enter a valid double number for '{0}':");

            return QueryValueFromUser(variable).AndThen(intValue => intValue.ToString(CultureInfo.InvariantCulture));
        }

        private Option<double> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, GetDoubleValidation())
                .Match(none: () => QueryValueFromUser(variable), some: ToDouble);
        }

        private static Option<double> ToDouble(string validatedDoubleString)
        {
            return Option.Some(double.Parse(validatedDoubleString));
        }

        private static IEnumerable<IValidation> GetDoubleValidation()
        {
            var dummy = 0.0;
            yield return new SimpleValidation(input => double.TryParse(input, out dummy), "Please enter true or false (no numeric input allowed).");
        }
    }
}