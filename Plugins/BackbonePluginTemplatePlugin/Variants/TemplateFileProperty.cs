using Messerli.MetaGeneratorAbstractions;

namespace Messerli.BackbonePluginTemplatePlugin.Variants
{
    public sealed class TemplateFileProperty
    {
        public TemplateFileProperty(
            IFileGenerator fileGenerator,
            string repositoryPath,
            string repositoryName)
        {
            FileGenerator = fileGenerator;
            RepositoryPath = repositoryPath;
            RepositoryName = repositoryName;
        }

        public IFileGenerator FileGenerator { get; }

        public string RepositoryPath { get; }

        public string RepositoryName { get; }
    }
}
