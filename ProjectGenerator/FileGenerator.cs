using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace Messerli.ProjectGenerator
{
    public class FileGenerator : IFileGenerator
    {
        private readonly IProjectInformationProvider _projectInformationProvider;
        private readonly IUserInputProvider _userInputProvider;
        private readonly StubbleBuilder _stubbleBuilder;

        public FileGenerator(IProjectInformationProvider projectInformationProvider, IUserInputProvider userInputProvider, StubbleBuilder stubbleBuilder)
        {
            _projectInformationProvider = projectInformationProvider;
            _userInputProvider = userInputProvider;
            _stubbleBuilder = stubbleBuilder;
        }

        public void FromTemplate(string templateName, string relativePath)
        {
            CreateMissingDirectories(AbsolutePath(relativePath));

            File.WriteAllText(AbsolutePath(relativePath), OutputFromTemplate(templateName));
        }

        private string AbsolutePath(string relativePath)
        {
            return Path.Combine(_projectInformationProvider.DestinationPath, relativePath);
        }

        private string OutputFromTemplate(string templateName)
        {
            var stubble = _stubbleBuilder
                .Configure(StubbleBuilderSettings)
                .Build();

            return stubble.Render(GetTemplate(templateName, Assembly.GetCallingAssembly()), _userInputProvider.View(), TemplateRenderSettings());
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

        private static string GetTemplate(string templateName, Assembly assembly)
        {
            if (assembly.GetManifestResourceNames().Contains(templateName))
            {
                return ReadTemplate1(templateName, assembly).Match(
                    none: () => throw new NotImplementedException(),
                    some: s => s);
            }

            throw new Exception($"There is no template resource with the name {templateName}");
        }

        private static Option<string> ReadTemplate1(string templateName, Assembly assembly)
        {
            using (var templateStream = assembly.GetManifestResourceStream(templateName))
            {
                if (templateStream != null)
                {
                    return Option.Some(ReadTemplate(templateStream));
                }
            }

            return Option<string>.None();
        }

        private static string ReadTemplate(Stream templateStream)
        {
            using (StreamReader reader = new StreamReader(templateStream))
            {
                return reader.ReadToEnd();
            }
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
