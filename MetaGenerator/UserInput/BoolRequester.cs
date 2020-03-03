using System;
using System.Collections.Generic;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class BoolRequester : AbstractVariableRequester
    {
        private static readonly HashSet<string> ValidTrueStrings = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true", "1", "yes", "y" };
        private static readonly HashSet<string> ValidFalseStrings = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "false", "0", "no", "n" };

        public BoolRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please enter true or false for '{0}':");

            return QueryValueFromUser(variable).Match(
                none: () => throw new NotImplementedException("cannot not happen"),
                some: boolValue => boolValue.ToString());
        }

        protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        {
            yield return new SimpleValidation(IsValidInput, "Please enter a valid 'yes' or 'no' value.");
        }

        private Option<bool> QueryValueFromUser(IUserInputDescription variable)
        {
            return ValidatedUserInput
                .GetValidatedValue(variable, RequesterValidations(variable))
                .Match(none: () => QueryValueFromUser(variable), some: ToBool);
        }

        private static bool IsValidInput(string input)
        {
            return ValidTrueStrings.Contains(input) || ValidFalseStrings.Contains(input);
        }

        private static Option<bool> ToBool(string validatedBoolString)
        {
            return validatedBoolString switch
            {
                _ when ValidTrueStrings.Contains(validatedBoolString) => Option.Some(true),
                _ when ValidFalseStrings.Contains(validatedBoolString) => Option.Some(false),
                _ => Option<bool>.None()
            };
        }
    }
}