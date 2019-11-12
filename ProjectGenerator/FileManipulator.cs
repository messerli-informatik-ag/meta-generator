using System;
using System.IO;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;
using Messerli.ProjectAbstractions.UserInput;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace Messerli.ProjectGenerator
{
    public class FileManipulator : IFileManipulator
    {
        private readonly IUserInputProvider _userInputProvider;
        private readonly ITemplateLoader _templateLoader;
        private readonly StubbleBuilder _stubbleBuilder;
        private readonly IConsoleWriter _consoleWriter;

        public FileManipulator(
            IUserInputProvider userInputProvider,
            ITemplateLoader templateLoader,
            StubbleBuilder stubbleBuilder,
            IConsoleWriter consoleWriter)
        {
            _userInputProvider = userInputProvider;
            _templateLoader = templateLoader;
            _stubbleBuilder = stubbleBuilder;
            _consoleWriter = consoleWriter;
        }

        public async Task AppendTemplate(string templateName, string filePath)
        {
            _consoleWriter.WriteLine($"Append template '{templateName}' to '{filePath}'");

            if (File.Exists(filePath) == false)
            {
                throw new Exception($"cannot append to file '{filePath}' it does not exist.");
            }

            using (var sw = File.AppendText(filePath))
            {
                await sw.WriteAsync(await OutputFromTemplate(templateName));
            }
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
    }
}
