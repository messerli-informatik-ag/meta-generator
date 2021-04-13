using System;
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
            => userArgument
                .Inspect(EchoVariable(variable))
                .SelectMany(Validate(variable, requesterValidations));

        public Option<string> GetValidatedValue(IUserInputDescription variable, IEnumerable<IValidation> requesterValidations)
        {
            _consoleWriter.WriteLine();
            _consoleWriter.Write($"{variable.VariableName}: ");

            return Validate(variable, requesterValidations)(_consoleReader.ReadLine());
        }

        public void WriteQuestion(IUserInputDescription variable, string defaultQuestion)
        {
            var question = variable.VariableQuestion.GetOrElse(defaultQuestion);

            _consoleWriter.WriteLine();
            _consoleWriter.WriteLine(FormatWithVariableName(question, variable.VariableName));
        }

        private Action<string> EchoVariable(IUserInputDescription variable)
            => argument
                =>
                {
                    _consoleWriter.WriteLine();
                    _consoleWriter.WriteLine($"{variable.VariableName}*: {argument}");
                };

        private Func<string, Option<string>> Validate(IUserInputDescription variable, IEnumerable<IValidation> requesterValidations)
            => userInput
                => variable
                    .Validations
                    .Concat(requesterValidations)
                    .Where(validation => validation.Validation(userInput) == false)
                    .Aggregate(Option.Some(userInput), (_, validation) => AggregateValidationErrors(validation, variable.VariableName));

        private Option<string> AggregateValidationErrors(IValidation validation, string variableName)
        {
            _consoleWriter.WriteLine(FormatWithVariableName(validation.Message, variableName).Pastel(Color.OrangeRed));

            return Option<string>.None();
        }

        private static string FormatWithVariableName(string format, string variableName)
            => format.Contains("{0}")
                ? string.Format(format, variableName)
                : format;
    }
}
