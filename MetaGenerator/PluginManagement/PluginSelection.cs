using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGenerator.UserInput;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Pastel;

namespace Messerli.MetaGenerator.PluginManagement
{
    internal class PluginSelection : IPluginSelection
    {
        private const int NoPluginsFound = 2;

        private readonly IConsoleWriter _consoleWriter;
        private readonly IGenerationSteps _generationSteps;
        private readonly IEnumerable<IMetaGenerator> _generators;
        private readonly SelectionRequester _selectionRequester;
        private readonly Func<UserInputDescriptionBuilder> _newUserInputDescriptionBuilder;
        private readonly IPluginRepository _pluginRepository;

        public PluginSelection(
            IConsoleWriter consoleWriter,
            IGenerationSteps generationSteps,
            IEnumerable<IMetaGenerator> generators,
            SelectionRequester selectionRequester,
            Func<UserInputDescriptionBuilder> newUserInputDescriptionBuilder,
            IPluginRepository pluginRepository)
        {
            _consoleWriter = consoleWriter;
            _generationSteps = generationSteps;
            _generators = generators;
            _selectionRequester = selectionRequester;
            _newUserInputDescriptionBuilder = newUserInputDescriptionBuilder;
            _pluginRepository = pluginRepository;
        }

        public async Task<int> StartPluginInteractive(InvocationContext context)
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
                    return await PrintPluginNotFound();
                }

                throw;
            }
        }

        public int StartPlugin(InvocationContext context, string generatorName)
            => _generationSteps.Execute(_generators.First(generator => generator.Name == generatorName), context);

        private async Task<int> PrintPluginNotFound()
        {
            _consoleWriter.WriteLine("No plugins found!".Pastel(Color.OrangeRed));
            _consoleWriter.WriteLine("--- available plugins ---");

            var plugins = await _pluginRepository
                .Plugins();

            plugins.ForEach(Console.WriteLine);

            return NoPluginsFound;
        }

        private IUserInputDescription ToSelection(IEnumerable<IMetaGenerator> metaGenerators)
            => _newUserInputDescriptionBuilder()
                .SetVariableName("GeneratorType")
                .SetVariableDescription("What do you want to generate?")
                .SetVariableType(VariableType.Selection)
                .SetVariableQuestion("What do you want to generate? Please select from the following options.")
                .SetSelectionValues(ToSelectionValues(metaGenerators))
                .Build();

        private List<SelectionValue> ToSelectionValues(IEnumerable<IMetaGenerator> metaGenerators)
            => metaGenerators
                .Select(metaGenerator => new SelectionValue { Value = metaGenerator.Name, Description = $"{metaGenerator.Description} ({metaGenerator.Name})" })
                .ToList();
    }
}
