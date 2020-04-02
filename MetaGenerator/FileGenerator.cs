using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace Messerli.MetaGenerator
{
    internal class FileGenerator : IFileGenerator
    {
        private readonly IVariableProvider _variableProvider;
        private readonly ITemplateLoader _templateLoader;
        private readonly StubbleBuilder _stubbleBuilder;
        private readonly IConsoleWriter _consoleWriter;

        public FileGenerator(
            IVariableProvider variableProvider,
            ITemplateLoader templateLoader,
            StubbleBuilder stubbleBuilder,
            IConsoleWriter consoleWriter)
        {
            _variableProvider = variableProvider;
            _templateLoader = templateLoader;
            _stubbleBuilder = stubbleBuilder;
            _consoleWriter = consoleWriter;
        }

        public async Task FromTemplate(string templateName, string destinationPath, Encoding encoding)
        {
            LogFileCreation(templateName, destinationPath);

            var content = await OutputFromTemplate(templateName);
            await FromTemplateContent(content, destinationPath, encoding);
        }

        public Task FromTemplateGlob(string glob, IDictionary<string, string> fileNameTemplateValues, Encoding encoding)
        {
            var globResults = _templateLoader.GetTemplatesFromGlob(glob);

            var fromTemplate = CurryFromTemplate(fileNameTemplateValues, encoding);
            var tasks = globResults.Select(fromTemplate);
            return Task.WhenAll(tasks);
        }

        private void LogFileCreation(string templateName, string destinationPath)
            => _consoleWriter.WriteLine($"Generate file from template '{templateName}' in '{destinationPath}'");

        private static Task FromTemplateContent(string templateContent, string destinationPath, Encoding encoding)
        {
            CreateMissingDirectories(destinationPath);
            return File.WriteAllTextAsync(destinationPath, templateContent, encoding);
        }

        private Func<Template, Task> CurryFromTemplate(IDictionary<string, string> fileNameTemplateValues, Encoding encoding)
            => template =>
            {
                var templateName = FillInFileNameTemplateValues(template.TemplateName, fileNameTemplateValues);
                var destinationPath = ConvertTemplateNameToDestinationPath(templateName);
                LogFileCreation(templateName, destinationPath);
                return FromTemplateContent(template.Content, destinationPath, encoding);
            };

        private static string ConvertTemplateNameToDestinationPath(string templateName)
            => templateName
                .Replace(".template", string.Empty)
                .Replace(".mustache", string.Empty);

        private static string FillInFileNameTemplateValues(string fileName, IDictionary<string, string> fileNameTemplateValues)
            => fileNameTemplateValues
                .Aggregate(fileName, FillInFileNameTemplateValue);

        private static string FillInFileNameTemplateValue(string fileName, KeyValuePair<string, string> fileNameTemplateValue)
            => fileName.Replace($"{{{fileNameTemplateValue.Key}}}", fileNameTemplateValue.Value);

        private async Task<string> OutputFromTemplate(string templateName)
        {
            var stubble = _stubbleBuilder
                .Configure(StubbleBuilderSettings)
                .Build();

            return await stubble.RenderAsync(_templateLoader.GetTemplate(templateName), _variableProvider.GetVariableValues(), TemplateRenderSettings());
        }

        private static void StubbleBuilderSettings(RendererSettingsBuilder settings)
            => settings.SetIgnoreCaseOnKeyLookup(false);

        private static RenderSettings TemplateRenderSettings()
            => new RenderSettings
            {
                ThrowOnDataMiss = true,
                SkipHtmlEncoding = true,
            };

        private static void CreateMissingDirectories(string path)
        {
            var folder = Path.GetDirectoryName(path);

            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}
