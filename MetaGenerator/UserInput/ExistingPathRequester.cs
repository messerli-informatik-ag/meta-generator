using System;
using System.Collections.Generic;
using System.IO;
using Funcky;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class ExistingPathRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;
        private readonly IEnumerable<IValidation> _requesterValidations = GetPathValidation();

        public ExistingPathRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public string RequestValue(IUserInputDescription variable, Option<string> userArgument)
        {
            return _validatedUserInput.ValidateArgument(variable, userArgument, _requesterValidations)
                .Match(() => InteractiveQuery(variable), Functional.Identity);
        }

        private string InteractiveQuery(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please enter a valid path which already exists '{0}':");

            return QueryValueFromUser(variable).Match(
                none: () => throw new NotImplementedException("cannot not happen"),
                some: Functional.Identity);
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, _requesterValidations)
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