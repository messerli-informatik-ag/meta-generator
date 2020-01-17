using System.Linq;
using Funcky.Monads;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class PathRequester : IVariableRequester
    {
        private readonly IValidatedUserInput _validatedUserInput;

        public PathRequester(IValidatedUserInput validatedUserInput)
        {
            _validatedUserInput = validatedUserInput;
        }

        public Option<string> RequestValue(IUserInputDescription variable)
        {
            _validatedUserInput.WriteQuestion(variable, "Please enter a valid path for '{0}':");

            return QueryValueFromUser(variable)
                .AndThen(v => v);
        }

        private Option<string> QueryValueFromUser(IUserInputDescription variable)
        {
            return _validatedUserInput
                .GetValidatedValue(variable, Enumerable.Empty<IValidation>())
                .Match(() => QueryValueFromUser(variable), Option.Some);
        }
    }
}