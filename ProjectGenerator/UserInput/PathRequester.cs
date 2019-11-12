using System;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class PathRequester : IVariableRequester
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IConsoleWriter _consoleWriter;

        public PathRequester(IConsoleReader consoleReader, IConsoleWriter consoleWriter)
        {
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            WriteQuestion(variable);

            return QueryValueFromUser(variable);
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return Option.Some(_consoleReader.ReadLine());
        }

        private void WriteQuestion(IUserInputDescription variable)
        {
            var question = variable.VariableQuestion
                           ?? $"Please enter a valid path'{variable.VariableName}':";

            _consoleWriter.WriteLine(question);
        }
    }
}