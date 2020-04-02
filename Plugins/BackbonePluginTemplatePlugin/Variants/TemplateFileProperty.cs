using Messerli.MetaGeneratorAbstractions;

namespace Messerli.BackbonePluginTemplatePlugin.Variants
{
    public sealed class TemplateFileProperty
    {
        public TemplateFileProperty(
            IFileGenerator fileGenerator,
            string solutionDirectory,
            string pluginName)
        {
            FileGenerator = fileGenerator;
            SolutionDirectory = solutionDirectory;
            PluginName = pluginName;
        }

        public IFileGenerator FileGenerator { get; }

        public string SolutionDirectory { get; }

        public string PluginName { get; }

        public string PluginPath => SolutionDirectory + PluginName;
    }
}
