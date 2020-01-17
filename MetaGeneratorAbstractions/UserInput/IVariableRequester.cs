using Funcky.Monads;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public interface IVariableRequester
    {
        Option<string> RequestValue(IUserInputDescription variable);
    }
}