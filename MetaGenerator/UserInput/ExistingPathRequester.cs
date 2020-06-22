using System.Collections.Generic;
using System.IO;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Messerli.MetaGenerator.UserInput.Utility;

namespace Messerli.MetaGenerator.UserInput
{
    internal class ExistingPathRequester : AbstractVariableRequester
    {
        public ExistingPathRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        {
            yield return new SimpleValidation(PathExists, "The path you have given does not exists, please enter an existing path:");
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please enter a valid path which already exists '{0}':");

            return Retry(() => QueryValueFromUser(variable));
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
            => ValidatedUserInput.GetValidatedValue(variable, RequesterValidations(variable));

        private static bool PathExists(string path) => Directory.Exists(path) || File.Exists(path);
    }
}
