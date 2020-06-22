using System.Collections.Generic;
using System.Globalization;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Messerli.MetaGenerator.UserInput.Utility;

namespace Messerli.MetaGenerator.UserInput
{
    internal class DoubleRequester : AbstractVariableRequester
    {
        public DoubleRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        {
            var dummy = 0.0;
            yield return new SimpleValidation(input => double.TryParse(input, out dummy), "Please enter true or false (no numeric input allowed).");
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please enter a valid double number for '{0}':");

            return Retry(() => QueryValueFromUser(variable)).ToString(CultureInfo.InvariantCulture);
        }

        private Option<double> QueryValueFromUser(IUserInputDescription variable)
            => ValidatedUserInput
                   .GetValidatedValue(variable, RequesterValidations(variable))
                   .SelectMany(ToDouble);

        private static Option<double> ToDouble(string validatedDoubleString)
            => Option.Some(double.Parse(validatedDoubleString));
    }
}
