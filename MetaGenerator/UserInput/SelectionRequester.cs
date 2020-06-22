using System;
using System.Collections.Generic;
using System.Linq;
using Funcky;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Messerli.MetaGenerator.UserInput.Utility;

namespace Messerli.MetaGenerator.UserInput
{
    internal class SelectionRequester : AbstractVariableRequester
    {
        private readonly IConsoleWriter _consoleWriter;

        public SelectionRequester(IValidatedUserInput validatedUserInput, IConsoleWriter consoleWriter)
            : base(validatedUserInput)
        {
            _consoleWriter = consoleWriter;
        }

        protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        {
            yield return new SimpleValidation(input => IsValuePossible(variable, input), $"Please select from the possible options between 1 and {ToHumanIndex(variable.VariableSelectionValues.Count - 1)}");
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            CheckForMissingOptions(variable);

            ValidatedUserInput.WriteQuestion(variable, "Please select one of the given values for '{0}':");
            WriteOptions(variable);

            return Retry(() => QueryValueFromUser(variable));
        }

        private static void CheckForMissingOptions(IUserInputDescription variable)
        {
            if (variable.VariableSelectionValues.Any() == false)
            {
                throw new ArgumentOutOfRangeException(nameof(variable));
            }
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
            => ValidatedUserInput
                   .GetValidatedValue(variable, RequesterValidations(variable))
                   .SelectMany(input => IndexToValue(input, variable));

        private static Option<string> IndexToValue(string input, IUserInputDescription variable)
        {
            var index = int.Parse(input);

            return Option.Some(variable.VariableSelectionValues[FromHumanIndex(index)].Value!);
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
                _consoleWriter.WriteLine($"{ToHumanIndex(index)}.) {selectionValue.Description}");
            }
        }

        private static int ToHumanIndex(int index) => index + 1;

        private static int FromHumanIndex(int index) => index - 1;
    }
}
