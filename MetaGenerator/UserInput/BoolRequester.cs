using System;
using System.Collections.Generic;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Funcky.Functional;

namespace Messerli.MetaGenerator.UserInput
{
    internal class BoolRequester : AbstractVariableRequester
    {
        private static readonly HashSet<string> ValidTrueStrings = new (StringComparer.OrdinalIgnoreCase) { "true", "1", "yes", "y" };
        private static readonly HashSet<string> ValidFalseStrings = new (StringComparer.OrdinalIgnoreCase) { "false", "0", "no", "n" };

        public BoolRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please enter true or false for '{0}':");

            return Retry(() => QueryValueFromUser(variable)).ToString();
        }

        protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        {
            yield return new SimpleValidation(IsValidInput, "Please enter a valid 'yes' or 'no' value.");
        }

        private Option<bool> QueryValueFromUser(IUserInputDescription variable)
            => ValidatedUserInput
                   .GetValidatedValue(variable, RequesterValidations(variable))
                   .SelectMany(ToBool);

        private static bool IsValidInput(string input)
            => ValidTrueStrings.Contains(input) || ValidFalseStrings.Contains(input);

        private static Option<bool> ToBool(string validatedBoolString)
            => validatedBoolString switch
            {
                _ when ValidTrueStrings.Contains(validatedBoolString) => Option.Some(true),
                _ when ValidFalseStrings.Contains(validatedBoolString) => Option.Some(false),
                _ => Option<bool>.None(),
            };
    }
}
