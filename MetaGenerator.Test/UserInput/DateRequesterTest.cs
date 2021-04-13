using System.Collections.Generic;
using System.Globalization;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGenerator.UserInput;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Moq;
using Xunit;

namespace Messerli.MetaGenerator.Test.UserInput
{
    public class DateRequesterTest
    {
        public DateRequesterTest()
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-CH", false);
        }

        [Fact]
        public void ReadingADateFromConsoleReturnsADate()
        {
            static bool IsNeeded() => true;

            var reader = new Mock<IConsoleReader>();
            var writer = new Mock<IConsoleWriter>();
            reader.SetupSequence(r => r.ReadLine())
                .Returns("none")
                .Returns("24.1.2009");

            var dateRequester = new DateRequester(new ValidatedUserInput(reader.Object, writer.Object));
            var validations = new List<IValidation>();
            var variableSelectionValues = Option<List<SelectionValue>>.None();
            var valueDescription = new UserInputDescription("myDate", "question?", "description", VariableType.Date, IsNeeded, variableSelectionValues, validations);

            Assert.Equal("24.01.2009", dateRequester.RequestValue(valueDescription, Option<string>.None()));
        }
    }
}
