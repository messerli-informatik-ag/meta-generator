using System;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class DateTimeRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;

        public DateTimeRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please a valid date and time for '{0}':");

            throw new NotImplementedException();
        }
    }
}