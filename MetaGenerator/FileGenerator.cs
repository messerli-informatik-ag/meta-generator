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

        public Task FromTemplate(string templatename, string destinationPath)
            => FromTemplate(templatename, destinationPath, Encoding.UTF8);

        public Task FromTemplate(string templateName, string destinationPath, Encoding encoding)
            => FromTemplate(templateName, _templateLoader.GetTemplate(templateName), destinationPath, encoding);

        public Task FromTemplateGlob(string glob, string destinationDirectory)
            => FromTemplateGlob(glob, destinationDirectory, new Dictionary<string, string>());

        public Task FromTemplateGlob(string glob, string destinationDirectory, IDictionary<string, string> fileNameTemplateValues)
            => FromTemplateGlob(glob, destinationDirectory, fileNameTemplateValues, Encoding.UTF8);

        public Task FromTemplateGlob(string glob, string destinationDirectory, IDictionary<string, string> fileNameTemplateValues, Encoding encoding)
        {
            var globResults = _templateLoader.GetTemplatesFromGlob(glob);
            var globResultsWithoutHardCodedPath = RemoveHardCodedPathPrefixFromTemplateNames(glob, globResults);

            var fromTemplate = CurryFromTemplate(fileNameTemplateValues, destinationDirectory, encoding);
            var tasks = globResultsWithoutHardCodedPath.Select(fromTemplate);
            return Task.WhenAll(tasks);
        }

        private static IEnumerable<Template> RemoveHardCodedPathPrefixFromTemplateNames(string glob, IEnumerable<Template> templates)
        {
            var variablePathBegin = glob.IndexOf("/*", StringComparison.Ordinal);
            return variablePathBegin <= 0
                ? templates
                : templates.Select(template =>
                    new Template(template.TemplateName.Substring(variablePathBegin), template.Content));
        }

        private async Task FromTemplate(string templateName, string templateContent, string destinationPath, Encoding encoding)
        {
            LogFileCreation(templateName, destinationPath);

            var content = await OutputFromTemplate(templateContent);
            await FromTemplateContent(content, destinationPath, encoding);
        }

        private void LogFileCreation(string templateName, string destinationPath)
            => _consoleWriter.WriteLine($"Generate file from template '{templateName}' in '{destinationPath}'");

        private static Task FromTemplateContent(string templateContent, string destinationPath, Encoding encoding)
        {
            CreateMissingDirectories(destinationPath);
            return File.WriteAllTextAsync(destinationPath, templateContent, encoding);
        }

        private Func<Template, Task> CurryFromTemplate(IDictionary<string, string> fileNameTemplateValues, string destinationDirectory, Encoding encoding)
            => template =>
            {
                var pathWithPlaceholders = ConvertTemplateNameToDestinationPath(template.TemplateName, destinationDirectory);
                var finalPath = FillInFileNameTemplateValues(pathWithPlaceholders, fileNameTemplateValues);
                return FromTemplate(template.TemplateName, template.Content, finalPath, encoding);
            };

        private static string ConvertTemplateNameToDestinationPath(string templateName, string destinationDirectory)
        {
            var stringBuilder = new StringBuilder(templateName);
            var firstDelimiter = templateName.IndexOf(Path.DirectorySeparatorChar);
            if (firstDelimiter > 0)
            {
                stringBuilder.Remove(0, firstDelimiter);
            }

            return stringBuilder
                .Insert(0, destinationDirectory)
                .Replace(".template", string.Empty)
                .Replace(".mustache", string.Empty)
                .ToString();
        }

        private static string FillInFileNameTemplateValues(string fileName, IDictionary<string, string> fileNameTemplateValues)
            => fileNameTemplateValues
                .Aggregate(fileName, FillInFileNameTemplateValue);

        private static string FillInFileNameTemplateValue(string fileName, KeyValuePair<string, string> fileNameTemplateValue)
            => fileName.Replace($"{{{fileNameTemplateValue.Key}}}", fileNameTemplateValue.Value);

        private async Task<string> OutputFromTemplate(string content)
        {
            var stubble = _stubbleBuilder
                .Configure(StubbleBuilderSettings)
                .Build();

            return await stubble.RenderAsync(content, _variableProvider.GetVariableValues(), TemplateRenderSettings());
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
