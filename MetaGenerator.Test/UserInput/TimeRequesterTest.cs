using System.Collections.Generic;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGenerator.UserInput;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Moq;
using Xunit;

namespace Messerli.MetaGenerator.Test.UserInput
{
    public class TimeRequesterTest
    {
        [Fact]
        public void ReadingATimeFromConsoleReturnsATime()
        {
            static bool IsNeeded() => true;

            var reader = new Mock<IConsoleReader>();
            var writer = new Mock<IConsoleWriter>();
            reader.SetupSequence(r => r.ReadLine())
                .Returns("none")
                .Returns("13:37:44");

            var dateRequester = new TimeRequester(new ValidatedUserInput(reader.Object, writer.Object));
            var validations = new List<IValidation>();
            var variableSelectionValues = Option<List<SelectionValue>>.None();
            var valueDescription = new UserInputDescription("myTime", "question?", "description", VariableType.Date, IsNeeded, variableSelectionValues, validations);

            Assert.Equal("13:37:44", dateRequester.RequestValue(valueDescription, Option<string>.None()));
        }
    }
}
