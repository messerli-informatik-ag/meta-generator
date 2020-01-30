using System;
using System.IO;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.VsSolution;
using Messerli.VsSolution.Model;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace Messerli.MetaGenerator
{
    internal class FileManipulator : IFileManipulator
    {
        private readonly IVariableProvider _variableProvider;
        private readonly ITemplateLoader _templateLoader;
        private readonly StubbleBuilder _stubbleBuilder;
        private readonly ISolutionLoader _solutionParser;
        private readonly IConsoleWriter _consoleWriter;

        public FileManipulator(
            IVariableProvider variableProvider,
            ITemplateLoader templateLoader,
            StubbleBuilder stubbleBuilder,
            ISolutionLoader solutionParser,
            IConsoleWriter consoleWriter)
        {
            _variableProvider = variableProvider;
            _templateLoader = templateLoader;
            _stubbleBuilder = stubbleBuilder;
            _solutionParser = solutionParser;
            _consoleWriter = consoleWriter;
        }

        public async Task AppendTemplate(string templateName, string filePath)
        {
            _consoleWriter.WriteLine($"Append template '{templateName}' to '{filePath}'");

            if (File.Exists(filePath) == false)
            {
                throw new Exception($"cannot append to file '{filePath}' it does not exist.");
            }

            await using var sw = File.AppendText(filePath);
            await sw.WriteAsync(await OutputFromTemplate(templateName));
        }

        public async Task AddProjectToSolution(string? solutionFolder, string projectName, string projectPath, string solutionPath, Guid? projectGuid)
        {
            var solution = await _solutionParser.Load(solutionPath);

            solution.AddProject(projectName, projectPath, ProjectType.Identifier.CSharpSdk, projectGuid);

            if (solutionFolder != null)
            {
                solution.AddNestedProject(solutionFolder, projectName);
            }

            await _solutionParser.Store(solutionPath, solution);
        }

        private async Task<string> OutputFromTemplate(string templateName)
        {
            var stubble = _stubbleBuilder
                .Configure(StubbleBuilderSettings)
                .Build();

            return await stubble.RenderAsync(_templateLoader.GetTemplate(templateName), _variableProvider.GetVariableValues(), TemplateRenderSettings());
        }

        private static void StubbleBuilderSettings(RendererSettingsBuilder settings)
        {
            settings.SetIgnoreCaseOnKeyLookup(false);
        }

        private static RenderSettings TemplateRenderSettings()
        {
            return new RenderSettings
            {
                ThrowOnDataMiss = true,
                SkipHtmlEncoding = true,
            };
        }
    }
}
