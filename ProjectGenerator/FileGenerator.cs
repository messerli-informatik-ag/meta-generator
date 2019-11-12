using System.IO;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;
using Messerli.ProjectAbstractions.UserInput;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace Messerli.ProjectGenerator
{
    public class FileGenerator : IFileGenerator
    {
        private readonly IProjectInformationProvider _projectInformationProvider;
        private readonly IUserInputProvider _userInputProvider;
        private readonly ITemplateLoader _templateLoader;
        private readonly StubbleBuilder _stubbleBuilder;
        private readonly IConsoleWriter _consoleWriter;

        public FileGenerator(
            IProjectInformationProvider projectInformationProvider,
            IUserInputProvider userInputProvider,
            ITemplateLoader templateLoader,
            StubbleBuilder stubbleBuilder,
            IConsoleWriter consoleWriter)
        {
            _projectInformationProvider = projectInformationProvider;
            _userInputProvider = userInputProvider;
            _templateLoader = templateLoader;
            _stubbleBuilder = stubbleBuilder;
            _consoleWriter = consoleWriter;
        }

        public async Task FromTemplate(string templateName, string relativePath)
        {
            CreateMissingDirectories(AbsolutePath(relativePath));

            _consoleWriter.WriteLine($"Generate file from template '{templateName}' in '{AbsolutePath(relativePath)}'");

            await File.WriteAllTextAsync(AbsolutePath(relativePath), await OutputFromTemplate(templateName));
        }

        private string AbsolutePath(string relativePath)
        {
            return Path.Combine(_projectInformationProvider.DestinationPath, relativePath);
        }

        private async Task<string> OutputFromTemplate(string templateName)
        {
            var stubble = _stubbleBuilder
                .Configure(StubbleBuilderSettings)
                .Build();

            return await stubble.RenderAsync(_templateLoader.GetTemplate(templateName), _userInputProvider.View(), TemplateRenderSettings());
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
