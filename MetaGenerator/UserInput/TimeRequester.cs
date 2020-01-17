﻿using System;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    public class TimeRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;

        public TimeRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please a valid time for '{0}':");

            throw new NotImplementedException();
        }
    }
}