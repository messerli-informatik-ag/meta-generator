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
    public class BoolRequesterTest
    {
        [Fact]
        public void ReadingABoolFromConsoleWorks()
        {
            static bool IsNeeded() => true;

            var reader = new Mock<IConsoleReader>();
            var writer = new Mock<IConsoleWriter>();
            reader.SetupSequence(r => r.ReadLine())
                .Returns("none")
                .Returns("false");

            var boolRequester = new BoolRequester(new ValidatedUserInput(reader.Object, writer.Object));
            var validations = new List<IValidation>();
            var variableSelectionValues = Option<List<SelectionValue>>.None();
            var valueDescription = new UserInputDescription("myBoolean", "question?", "description", VariableType.Bool, IsNeeded, variableSelectionValues, validations);

            Assert.Equal("False", boolRequester.RequestValue(valueDescription, Option<string>.None()));
        }
    }
}
