using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Drawing;
using System.Linq;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGenerator.UserInput;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Pastel;

namespace Messerli.MetaGenerator
{
    internal class PluginSelection : IPluginSelection
    {
        private const int NoPluginsFound = 2;

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

        public int StartPluginInteractive(InvocationContext context)
        {
            try
            {
                var generatorName = _selectionRequester
                    .RequestValue(ToSelection(_generators.OrderBy(g => g.Name)), Option<string>.None());

                return StartPlugin(context, generatorName);
            }
            catch (ArgumentOutOfRangeException argumentOutOfRange)
            {
                if (argumentOutOfRange.ParamName == "variable")
                {
                    return PrintPluginNotFound();
                }

                throw;
            }
        }

        public int StartPlugin(InvocationContext context, string generatorName)
        {
            return _generationSteps.Execute(_generators.First(generator => generator.Name == generatorName), context);
        }

        private int PrintPluginNotFound()
        {
            _consoleWriter.WriteLine("No plugins found!".Pastel(Color.OrangeRed));
            _consoleWriter.WriteLine("<TODO> give help");

            return NoPluginsFound;
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
