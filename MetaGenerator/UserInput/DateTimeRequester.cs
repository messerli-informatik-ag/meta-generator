using System;
using System.Collections.Generic;
using Funcky;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.UserInput;
using static Funcky.Functional;

namespace Messerli.MetaGenerator.UserInput
{
    internal class DateTimeRequester : AbstractVariableRequester
    {
        public DateTimeRequester(IValidatedUserInput validatedUserInput)
            : base(validatedUserInput)
        {
        }

        protected override string InteractiveQuery(IUserInputDescription variable)
        {
            ValidatedUserInput.WriteQuestion(variable, "Please enter a valid date and time for '{0}':");

            return Retry(() => QueryValueFromUser(variable)).ToString();
        }

        protected override IEnumerable<IValidation> RequesterValidations(IUserInputDescription variable)
            => Sequence.Return(SimpleValidation.Create(IsValidInput, $"Please enter a valid date and time (like: {new DateTime(2000, 1, 1)})."));

        private bool IsValidInput(string input)
            => input.ParseDateTimeOrNone().Match(none: false, some: True);

        private Option<DateTime> QueryValueFromUser(IUserInputDescription variable)
            => ValidatedUserInput
                .GetValidatedValue(variable, RequesterValidations(variable))
                .SelectMany(ToDate);

        private Option<DateTime> ToDate(string input)
            => input
                .ParseDateTimeOrNone();
    }
}
