using System.Collections.Generic;
using System.IO;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class ExistingPathRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;

        public ExistingPathRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please enter a valid path which already exists '{0}':");

            return QueryValueFromUser(variable);
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, GetPathValidation())
                .Match(none: () => QueryValueFromUser(variable), some: Option.Some);
        }

        private static IEnumerable<IValidation> GetPathValidation()
        {
            yield return new SimpleValidation(PathExists, "The path you have given does not exists, please enter an existing path:");
        }

        private static bool PathExists(string path)
        {
            return Directory.Exists(path) || File.Exists(path);
        }
    }
}