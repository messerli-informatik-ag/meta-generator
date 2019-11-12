using System;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class SelectionRequester : IVariableRequester
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IConsoleWriter _consoleWriter;

        public SelectionRequester(IConsoleReader consoleReader, IConsoleWriter consoleWriter)
        {
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            CheckForMissingOptions(variable);

            WriteQuestion(variable);
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
            return QueryOptionValue(variable).Match(() => RetryQueryValueFromUser(variable), Option.Some);
        }

        private Option<string> RetryQueryValueFromUser(IUserInputDescription variable)
        {
            _consoleWriter.WriteLine($"Please select from the possible options between 1 and {ToHumandIndex(variable.VariableSelectionValues.Count - 1)}");

            return QueryValueFromUser(variable);
        }

        private Option<string> QueryOptionValue(IUserInputDescription variable)
        {
            var maybeValue = _consoleReader.ReadLine().TryParseInt();

            return IsValuePossible(variable, maybeValue)
                ? maybeValue.AndThen(index => variable.VariableSelectionValues[FromHumandIndex(index)].Value!)
                : Option<string>.None();
        }

        private static bool IsValuePossible(IUserInputDescription variable, Option<int> maybeValue)
        {
            return maybeValue.Match(
                false,
                value => value > 0
                         && value <= variable.VariableSelectionValues.Count);
        }

        private void WriteQuestion(IUserInputDescription variable)
        {
            var question = variable.VariableQuestion
                           ?? $"Please select one of the given values for '{variable.VariableName}':";

            _consoleWriter.WriteLine(question);
        }

        private void WriteOptions(IUserInputDescription variable)
        {
            foreach (var (selectionValue, index) in variable.VariableSelectionValues.Select((selectionValue, index) =>
                (selectionValue, index)))
            {
                _consoleWriter.WriteLine($"{ToHumandIndex(index)}.) {selectionValue.Value} ({selectionValue.Description})");
            }
        }

        private int ToHumandIndex(int index) => index + 1;

        private int FromHumandIndex(int index) => index - 1;
    }
}