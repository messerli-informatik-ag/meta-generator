using System.Collections.Generic;
using System.Globalization;
using Funcky;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Funcky.Functional;

namespace Messerli.MetaGenerator.UserInput;

internal class DoubleRequester : AbstractVariableRequester
{
    public DoubleRequester(IValidatedUserInput validatedUserInput)
        : base(validatedUserInput)
    {
    }

    protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        => Sequence.Return(SimpleValidation.Create(IsValidInput, "Please enter a valid double value."));

    protected override string InteractiveQuery(IUserInputDescription variable)
    {
        ValidatedUserInput.WriteQuestion(variable, "Please enter a valid double number for '{0}':");

        return Retry(() => QueryValueFromUser(variable)).ToString(CultureInfo.InvariantCulture);
    }

    private bool IsValidInput(string input)
        => input.ParseDoubleOrNone().Match(none: false, some: True);

    private Option<double> QueryValueFromUser(IUserInputDescription variable)
        => ValidatedUserInput
            .GetValidatedValue(variable, RequesterValidations(variable))
            .SelectMany(ToDouble);

    private static Option<double> ToDouble(string validatedDoubleString)
        => Option.Some(double.Parse(validatedDoubleString));
}
