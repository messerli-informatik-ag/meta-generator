using System.IO;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class ExistingPathRequester : IVariableRequester
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IConsoleWriter _consoleWriter;

        public ExistingPathRequester(IConsoleReader consoleReader, IConsoleWriter consoleWriter)
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
            return QueryPath().Match(() => RetryQueryValueFromUser(variable), Option.Some);
        }

        private Option<string> QueryPath()
        {
            var path = _consoleReader.ReadLine();

            return PathExists(path)
                ? Option.Some(path)
                : Option<string>.None();
        }

        private Option<string> RetryQueryValueFromUser(IUserInputDescription variable)
        {
            _consoleWriter.WriteLine("The path you have given does not exists, please enter an existing path:");

            return QueryValueFromUser(variable);
        }

        private bool PathExists(string path)
        {
            return Directory.Exists(path) || File.Exists(path);
        }

        private void WriteQuestion(IUserInputDescription variable)
        {
            var question = variable.VariableQuestion
                           ?? $"Please enter a valid path which already exists '{variable.VariableName}':";

            _consoleWriter.WriteLine(question);
        }
    }
}