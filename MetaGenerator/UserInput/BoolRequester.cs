using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class BoolRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;
        private static readonly HashSet<string> ValidTrueStrings = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true", "1", "yes", "y" };
        private static readonly HashSet<string> ValidFalseStrings = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "false", "0", "no", "n" };

        public BoolRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public string RequestValue(IUserInputDescription variable, Option<string> userArgument)
        {
            return _validatedUserInput.ValidateArgument(variable, userArgument, GetBoolValidation())
                .Match(() => InteractiveQuery(variable), Functional.Identity);
        }

        private string InteractiveQuery(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please enter true or false for '{0}':");

            return QueryValueFromUser(variable).Match(
                none: () => throw new NotImplementedException("cannot not happen"),
                some: boolValue => boolValue.ToString());
        }

        private Option<bool> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, GetBoolValidation())
                .Match(none: () => QueryValueFromUser(variable), some: ToBool);
        }

        private static IEnumerable<IValidation> GetBoolValidation()
        {
            yield return new SimpleValidation(IsValidInput, "Please enter a valid 'yes' or 'no' value.");
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