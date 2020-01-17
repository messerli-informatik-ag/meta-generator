using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class SelectionRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;
        private readonly IConsoleWriter _consoleWriter;

        public SelectionRequester(IValidatedUserInput validatedUserInput, IConsoleWriter consoleWriter)
        {
            _validatedUserInput = validatedUserInput;
            _consoleWriter = consoleWriter;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            CheckForMissingOptions(variable);

            _validatedUserInput.WriteQuestion(variable, "Please select one of the given values for '{0}':");
            WriteOptions(variable);

            return QueryValueFromUser(variable);
        }

        private static void CheckForMissingOptions(IUserInputDescription variable)
        {
            if (variable.VariableSelectionValues.Any() == false)
            {
                throw new Exception("There are no options to chose from...");
            }
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, GetSelectionValidation(variable))
                .Match(() => QueryValueFromUser(variable), input => IndexToValue(input, variable));
        }

        private Option<string> IndexToValue(string input, IUserInputDescription variable)
        {
            var index = int.Parse(input);

            return Option.Some(variable.VariableSelectionValues[FromHumandIndex(index)].Value!);
        }

        private static IEnumerable<IValidation> GetSelectionValidation(IUserInputDescription variable)
        {
            yield return new SimpleValidation(input => IsValuePossible(variable, input), $"Please select from the possible options between 1 and {ToHumandIndex(variable.VariableSelectionValues.Count - 1)}");
        }

        private static bool IsValuePossible(IUserInputDescription variable, string input)
        {
            var maybeValue = input.TryParseInt();

            return maybeValue.Match(
                false,
                value => value > 0
                         && value <= variable.VariableSelectionValues.Count);
        }

        private void WriteOptions(IUserInputDescription variable)
        {
            foreach (var (selectionValue, index) in variable.VariableSelectionValues.Select((selectionValue, index) =>
                (selectionValue, index)))
            {
                _consoleWriter.WriteLine($"{ToHumandIndex(index)}.) {selectionValue.Value} ({selectionValue.Description})");
            }
        }

        private static int ToHumandIndex(int index) => index + 1;

        private static int FromHumandIndex(int index) => index - 1;
    }
}