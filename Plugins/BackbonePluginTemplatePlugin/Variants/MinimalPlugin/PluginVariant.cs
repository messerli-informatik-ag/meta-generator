using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Messerli.BackbonePluginTemplatePlugin.Variants.MinimalPlugin
{
    public class PluginVariant : IPluginVariant
    {
        private const string TestFolder = "Test";

        private readonly TemplateFileProperty _templateFileProperty;

        public PluginVariant(TemplateFileProperty templateFileProperty)
        {
            _templateFileProperty = templateFileProperty;
        }

        public List<Task> CreateTemplateFiles()
            => new List<Task>
            {
                _templateFileProperty.FileGenerator.FromTemplate(Template.ProjectFile, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, $"{_templateFileProperty.RepositoryName}.csproj"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.PluginSource, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, "Plugin.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(Template.TestProjectFile, Path.Combine(_templateFileProperty.RepositoryPath, $"{_templateFileProperty.RepositoryName}.{TestFolder}", $"{_templateFileProperty.RepositoryName}.csproj"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.IntegrationTestSource, Path.Combine(_templateFileProperty.RepositoryPath, $"{_templateFileProperty.RepositoryName}.{TestFolder}", "IntegrationTests.cs"), Encoding.UTF8),
            };
    }
}
