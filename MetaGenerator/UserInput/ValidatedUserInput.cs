using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Pastel;

namespace Messerli.MetaGenerator.UserInput
{
    internal class ValidatedUserInput : IValidatedUserInput
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IConsoleWriter _consoleWriter;

        public ValidatedUserInput(IConsoleReader consoleReader, IConsoleWriter consoleWriter)
        {
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        public Option<string> ValidateArgument(IUserInputDescription variable, Option<string> userArgument, IEnumerable<IValidation> requesterValidations)
        {
            return userArgument.Match(
                none: Option<string>.None,
                some: argument =>
                {
                    _consoleWriter.WriteLine();
                    _consoleWriter.WriteLine($"{variable.VariableName}*: {argument}");

                    return Validate(variable, argument, requesterValidations);
                });
        }

        public Option<string> GetValidatedValue(IUserInputDescription variable, IEnumerable<IValidation> requesterValidations)
        {
            _consoleWriter.WriteLine();
            _consoleWriter.Write($"{variable.VariableName}: ");

            return Validate(variable, _consoleReader.ReadLine(), requesterValidations);
        }

        public void WriteQuestion(IUserInputDescription variable, string defaultQuestion)
        {
            var question = variable.VariableQuestion ?? defaultQuestion;

            _consoleWriter.WriteLine();
            _consoleWriter.WriteLine(FormatWithVariableName(question, variable.VariableName));
        }

        private Option<string> Validate(IUserInputDescription variable, string userInput, IEnumerable<IValidation> requesterValidations)
        {
            return variable
                .Validations
                .Concat(requesterValidations)
                .Where(validation => validation.Validation(userInput) == false)
                .Aggregate(Option.Some(userInput), (_, validation) => AggregateValidationErrors(validation, variable.VariableName));
        }

        private Option<string> AggregateValidationErrors(IValidation validation, string variableName)
        {
            _consoleWriter.WriteLine(FormatWithVariableName(validation.Message, variableName).Pastel(Color.OrangeRed));

            return Option<string>.None();
        }

        private static string FormatWithVariableName(string format, string variableName)
        {
            return format.Contains("{0}")
                ? string.Format(format, variableName)
                : format;
        }
    }
}
