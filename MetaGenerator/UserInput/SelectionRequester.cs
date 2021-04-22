using System;
using System.Collections.Generic;
using System.Linq;
using Funcky;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Funcky.Functional;

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
            => Sequence.Return(SimpleValidation.Create(IsValuePossible(variable), $"Please select from the possible options between 1 and {ToHumanIndex(variable.VariableSelectionValues.Count - 1)}"));

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            AtLeastOneValueAvailable(variable);

            ValidatedUserInput.WriteQuestion(variable, "Please select one of the given values for '{0}':");
            WriteOptions(variable);

            return Retry(() => QueryValueFromUser(variable));
        }

        private static void AtLeastOneValueAvailable(IUserInputDescription variable)
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

        private static Func<string, bool> IsValuePossible(IUserInputDescription variable)
            => input
                =>
                {
                    var maybeValue = input.ParseIntOrNone();

                    return maybeValue.Match(
                        false,
                        value => value > 0
                                 && value <= variable.VariableSelectionValues.Count);
                };

        private void WriteOptions(IUserInputDescription variable)
            => variable
                .VariableSelectionValues
                .WithIndex()
                .ForEach(WriteOption);

        private void WriteOption(ValueWithIndex<SelectionValue> selection)
            => _consoleWriter.WriteLine($"{ToHumanIndex(selection.Index)}.) {selection.Value.Description}");

        private static int ToHumanIndex(int index) => index + 1;

        private static int FromHumanIndex(int index) => index - 1;
    }
}
