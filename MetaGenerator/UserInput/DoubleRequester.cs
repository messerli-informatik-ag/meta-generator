using System;
using System.Collections.Generic;
using System.Globalization;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class DoubleRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;
        private readonly IEnumerable<IValidation> _requesterValidations = GetDoubleValidation();

        public DoubleRequester(IValidatedUserInput validatedUserInput)
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
            _validatedUserInput.WriteQuestion(variable, "Please enter a valid double number for '{0}':");

            return QueryValueFromUser(variable).Match(
                none: () => throw new NotImplementedException("cannot happen"),
                some: intValue => intValue.ToString(CultureInfo.InvariantCulture));
        }

        private Option<double> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, _requesterValidations)
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