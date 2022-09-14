using System;
using System.Collections.Generic;
using Funcky;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Funcky.Functional;

namespace Messerli.MetaGenerator.UserInput;

internal class TimeRequester : AbstractVariableRequester
{
    public TimeRequester(IValidatedUserInput validatedUserInput)
        : base(validatedUserInput)
    {
    }

    protected override string InteractiveQuery(IUserInputDescription variable)
    {
        ValidatedUserInput.WriteQuestion(variable, "Please enter a valid time for '{0}':");

        return Retry(() => QueryValueFromUser(variable)).ToString();
    }

    protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
        => Sequence.Return(SimpleValidation.Create(IsValidInput, $"Please enter a valid time (like: {new TimeSpan(13, 37, 42)})."));

    private bool IsValidInput(string input)
        => input.ParseDateTimeOrNone().Match(none: false, some: True);

    private Option<TimeSpan> QueryValueFromUser(IUserInputDescription variable)
        => ValidatedUserInput
            .GetValidatedValue(variable, RequesterValidations(variable))
            .SelectMany(ToDate);

    private Option<TimeSpan> ToDate(string input)
        => input
            .ParseTimeSpanOrNone();
}
