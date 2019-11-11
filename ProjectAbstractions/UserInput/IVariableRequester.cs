using Funcky.Monads;

namespace Messerli.ProjectAbstractions.UserInput
{
    public interface IVariableRequester
    {
        Option<string> RequestValue(IUserInputDescription variable);
    }
}