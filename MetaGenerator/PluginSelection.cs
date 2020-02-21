using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGenerator.UserInput;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator
{
    internal class PluginSelection : IPluginSelection
    {
        private readonly IConsoleWriter _consoleWriter;
        private readonly IGenerationSteps _generationSteps;
        private readonly IEnumerable<IMetaGenerator> _generators;
        private readonly SelectionRequester _selectionRequester;
        private readonly Func<UserInputDescriptionBuilder> _newUserInputDescriptionBuilder;

        public PluginSelection(
            IConsoleWriter consoleWriter,
            IGenerationSteps generationSteps,
            IEnumerable<IMetaGenerator> generators,
            SelectionRequester selectionRequester,
            Func<UserInputDescriptionBuilder> newUserInputDescriptionBuilder)
        {
            _consoleWriter = consoleWriter;
            _generationSteps = generationSteps;
            _generators = generators;
            _selectionRequester = selectionRequester;
            _newUserInputDescriptionBuilder = newUserInputDescriptionBuilder;
        }

        public void StartPluginInteractive(InvocationContext context)
        {
            var generatorName = _selectionRequester
                .RequestValue(ToSelection(_generators.OrderBy(g => g.Name)), Option<string>.None());

            StartPlugin(context, generatorName);
        }

        public void StartPlugin(InvocationContext context, string generatorName)
        {
            var generator = _generators.First(generator => generator.Name == generatorName);

            var result = _generationSteps.Execute(generator, context);
        }

        private IUserInputDescription ToSelection(IEnumerable<IMetaGenerator> metaGenerators)
        {
            return _newUserInputDescriptionBuilder()
                .SetVariableName("GeneratorType")
                .SetVariableDescription("What do you want to generate?")
                .SetVariableType(VariableType.Selection)
                .SetVariableQuestion("What do you want to generate? Please select from the following options.")
                .SetSelectionValues(ToSelectionValues(metaGenerators))
                .Build();
        }

        private List<SelectionValue> ToSelectionValues(IEnumerable<IMetaGenerator> metaGenerators)
        {
            return metaGenerators
                .Select(metaGenerator => new SelectionValue { Value = metaGenerator.Name, Description = $"{metaGenerator.Description} ({metaGenerator.Name})" })
                .ToList();
        }
    }
}
