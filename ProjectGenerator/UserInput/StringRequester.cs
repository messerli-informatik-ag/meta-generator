using System.Linq;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class StringRequester : IVariableRequester
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IConsoleWriter _consoleWriter;

        public StringRequester(IConsoleReader consoleReader, IConsoleWriter consoleWriter)
        {
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            WriteQuestion(variable);

            return QueryValueFromUser(variable)
                .AndThen(v => v);
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return ValidateInput(variable)
                .Match(() => RetryQueryValueFromUser(variable), Option.Some);
        }

        private Option<string> RetryQueryValueFromUser(IUserInputDescription variable)
        {
            return QueryValueFromUser(variable);
        }

        private Option<string> ValidateInput(IUserInputDescription variable)
        {
            var input = _consoleReader.ReadLine();

            return variable
                .Validations
                .Where(validation => validation.Validation(input) == false)
                .Aggregate(Option.Some(input), AggregateValidationErrors);
        }

        private Option<string> AggregateValidationErrors(Option<string> input, IValidation validation)
        {
            _consoleWriter.WriteLine(validation.Message);

            return Option<string>.None();
        }

        private void WriteQuestion(IUserInputDescription variable)
        {
            var question = variable.VariableQuestion
                           ?? $"Please enter a value for '{variable.VariableName}':";

            _consoleWriter.WriteLine(question);
        }
    }
}