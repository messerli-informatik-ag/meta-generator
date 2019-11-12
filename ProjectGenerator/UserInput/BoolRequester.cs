using System;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class BoolRequester : IVariableRequester
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IConsoleWriter _consoleWriter;

        public BoolRequester(IConsoleReader consoleReader, IConsoleWriter consoleWriter)
        {
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            WriteQuestion(variable);

            return QueryValueFromUser(variable).AndThen(boolValue => boolValue.ToString());
        }

        private Option<bool> QueryValueFromUser(IUserInputDescription variable)
        {
            return _consoleReader
                .ReadLine()
                .TryParseBoolean()
                .Match(() => RetryQueryValueFromUser(variable), Option.Some);
        }

        private Option<bool> RetryQueryValueFromUser(IUserInputDescription variable)
        {
            _consoleWriter.WriteLine("Please enter true or false (no numeric input allowed).");

            return QueryValueFromUser(variable);
        }

        private void WriteQuestion(IUserInputDescription variable)
        {
            var question = variable.VariableQuestion
                           ?? $"Please enter true or false for '{variable.VariableName}':";

            _consoleWriter.WriteLine(question);
        }
    }
}