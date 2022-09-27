using System.Collections.Generic;
using Funcky;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Funcky.Functional;

namespace Messerli.MetaGenerator.UserInput;

internal class IntegerRequester : AbstractVariableRequester
{
    public IntegerRequester(IValidatedUserInput validatedUserInput)
        : base(validatedUserInput)
    {
    }

    protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        => Sequence.Return(SimpleValidation.Create(IsValidInput, "Please enter a valid integer value."));

    protected override string InteractiveQuery(IUserInputDescription variable)
    {
        ValidatedUserInput.WriteQuestion(variable, "Please enter a valid integer for '{0}':");

        return Retry(() => QueryValueFromUser(variable)).ToString();
    }

    private static bool IsValidInput(string input)
        => input.ParseInt32OrNone().Match(none: false, some: True);

    private Option<int> QueryValueFromUser(IUserInputDescription variable)
        => ValidatedUserInput
            .GetValidatedValue(variable, RequesterValidations(variable))
            .SelectMany(ToInteger);

    private static Option<int> ToInteger(string validatedIntegerString)
        => Option.Some(int.Parse(validatedIntegerString));
}
