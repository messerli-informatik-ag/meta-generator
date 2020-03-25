﻿using System.Collections.Generic;
using Funcky;
using Funcky.Monads;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public abstract class AbstractVariableRequester
    {
        protected AbstractVariableRequester(IValidatedUserInput validatedUserInput)
        {
            ValidatedUserInput = validatedUserInput;
        }

        protected IValidatedUserInput ValidatedUserInput { get; }

        public string RequestValue(IUserInputDescription variable, Option<string> userArgument)
        {
            return ValidatedUserInput.ValidateArgument(variable, userArgument, RequesterValidations(variable))
                .Match(() => InteractiveQuery(variable), Functional.Identity);
        }

        protected abstract string InteractiveQuery(IUserInputDescription variable);

        protected abstract IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable);
    }
}