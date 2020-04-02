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
                _templateFileProperty.FileGenerator.FromTemplate(Template.ProjectFile, Path.Combine(_templateFileProperty.PluginPath, $"{_templateFileProperty.PluginName}.csproj"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.PluginSource, Path.Combine(_templateFileProperty.PluginPath, "Plugin.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(Template.TestProjectFile, Path.Combine(_templateFileProperty.SolutionDirectory, $"{_templateFileProperty.PluginName}.{TestFolder}", $"{_templateFileProperty.PluginName}.csproj"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.IntegrationTestSource, Path.Combine(_templateFileProperty.SolutionDirectory, $"{_templateFileProperty.PluginName}.{TestFolder}", "IntegrationTests.cs"), Encoding.UTF8),
            };
    }
}
