using Funcky.Monads;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public interface IVariableRequester
    {
        string RequestValue(IUserInputDescription variable, Option<string> userArgument);
    }
}