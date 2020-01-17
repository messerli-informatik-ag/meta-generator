using System;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class DateRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;

        public DateRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please a valid date for '{0}':");

            throw new NotImplementedException();
        }
    }
}