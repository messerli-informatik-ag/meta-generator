using System.IO;
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

        public async Task FromTemplate(string templateName, string path, Encoding encoding)
        {
            CreateMissingDirectories(path);

            _consoleWriter.WriteLine($"Generate file from template '{templateName}' in '{path}'");

            await File.WriteAllTextAsync(path, await OutputFromTemplate(templateName), encoding);
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

        private void CreateMissingDirectories(string path)
        {
            var folder = Path.GetDirectoryName(path);

            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}
