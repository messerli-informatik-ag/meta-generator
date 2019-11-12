using System;
using System.Globalization;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class DoubleRequester : IVariableRequester
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IConsoleWriter _consoleWriter;

        public DoubleRequester(IConsoleReader consoleReader, IConsoleWriter consoleWriter)
        {
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            WriteQuestion(variable);

            return QueryValueFromUser(variable).AndThen(intValue => intValue.ToString(CultureInfo.InvariantCulture));
        }

        private void WriteQuestion(IUserInputDescription variable)
        {
            var question = variable.VariableQuestion
                           ?? "Please only enter a valid double number.";

            _consoleWriter.WriteLine(question);
        }

        private Option<double> QueryValueFromUser(IUserInputDescription variable)
        {
            return _consoleReader
                .ReadLine()
                .TryParseDouble()
                .Match(() => RetryQueryValueFromUser(variable), Option.Some);
        }

        private Option<double> RetryQueryValueFromUser(IUserInputDescription variable)
        {
            _consoleWriter.WriteLine($"Please enter a valid double number for '{variable.VariableName}'.");

            return QueryValueFromUser(variable);
        }
    }
}