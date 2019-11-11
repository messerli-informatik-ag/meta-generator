using System;
using Funcky.Monads;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class ExistingPathRequester : IVariableRequester
    {
        public Option<string> RequestValue(IUserInputDescription variable)
        {
            throw new NotImplementedException();
        }
    }
}