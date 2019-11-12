using System;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class IntegerRequester : IVariableRequester
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IConsoleWriter _consoleWriter;

        public IntegerRequester(IConsoleReader consoleReader, IConsoleWriter consoleWriter)
        {
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            WriteQuestion(variable);

            return QueryValueFromUser(variable).AndThen(intValue => intValue.ToString());
        }

        private Option<int> QueryValueFromUser(IUserInputDescription variable)
        {
            return _consoleReader
                .ReadLine()
                .TryParseInt()
                .Match(() => RetryQueryValueFromUser(variable), Option.Some);
        }

        private Option<int> RetryQueryValueFromUser(IUserInputDescription variable)
        {
            _consoleWriter.WriteLine("Please only enter an integer number.");

            return QueryValueFromUser(variable);
        }

        private void WriteQuestion(IUserInputDescription variable)
        {
            var question = variable.VariableQuestion
                           ?? $"Please enter a valid integer for '{variable.VariableName}':";

            _consoleWriter.WriteLine(question);
        }
    }
}