using System;
using System.Collections.Generic;
using System.IO;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

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

            return QueryValueFromUser(variable).Match(
                none: () => throw new NotImplementedException("cannot not happen"),
                some: Functional.Identity);
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return ValidatedUserInput
                .GetValidatedValue(variable, RequesterValidations(variable))
                .Match(none: () => QueryValueFromUser(variable), some: Option.Some);
        }

        private static bool PathExists(string path)
        {
            return Directory.Exists(path) || File.Exists(path);
        }
    }
}