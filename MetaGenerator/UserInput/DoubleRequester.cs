using System;
using System.Collections.Generic;
using System.Globalization;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

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

            return QueryValueFromUser(variable).Select(DoubleToString).GetOrElse(
                () => throw new NotImplementedException("cannot happen"));
        }

        private static string DoubleToString(double value) => value.ToString(CultureInfo.InvariantCulture);

        private Option<double> QueryValueFromUser(IUserInputDescription variable)
        {
            return ValidatedUserInput
                .GetValidatedValue(variable, RequesterValidations(variable))
                .Match(none: () => QueryValueFromUser(variable), some: ToDouble);
        }

        private static Option<double> ToDouble(string validatedDoubleString)
        {
            return Option.Some(double.Parse(validatedDoubleString));
        }
    }
}
