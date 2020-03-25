using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Messerli.BackbonePluginTemplatePlugin.Variants;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.BackbonePluginTemplatePlugin
{
    public class BackbonePluginTemplatePluginGenerator : IMetaGenerator
    {
        private const string RepositoryNameVariable = "RepositoryName";
        private const string PluginVariantVariable = "PluginVariant";
        private const string TargetPath = "TargetPath";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;

        public BackbonePluginTemplatePluginGenerator(
            IConsoleWriter consoleWriter,
            IFileGenerator fileGenerator,
            IUserInputProvider userInputProvider)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
        }

        public string Description => "Create a Messerli backbone plugin template plugin";

        public string Name => "backbone-plugin-template-plugin";

        public void Register()
            => _userInputProvider.RegisterVariablesFromTemplate(Template.VariableDeclarations);

        public void Prepare()
        {
        }

        public void Generate()
        {
            _consoleWriter.WriteLine($"Creating Plugin: {RepositoryName()}");
            var tasks = CreatePluginVariant(BackbonePluginVariant())
                .CreateTemplateFiles();

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
        }

        private string RepositoryName()
            => _userInputProvider.Value(RepositoryNameVariable);

        private VariantType BackbonePluginVariant()
            => (VariantType)int.Parse(_userInputProvider.Value(PluginVariantVariable));

        private string RepositoryPath()
            => Path.Combine(
                _userInputProvider.Value(TargetPath),
                RepositoryName());

        private IPluginVariant CreatePluginVariant(VariantType variant)
            => variant switch
            {
                VariantType.MinimalPluginTemplate => new Variants.MinimalPluginTemplate.PluginVariant(CreateTemplateFileProperty()),
                VariantType.PluginTemplate => new Variants.PluginTemplate.PluginVariant(CreateTemplateFileProperty()),
                VariantType.DatabaseAccessPluginTemplate => new Variants.DatabaseAccessPluginTemplate.PluginVariant(CreateTemplateFileProperty()),
                _ => throw new InvalidOperationException(),
            };

        private TemplateFileProperty CreateTemplateFileProperty()
            => new TemplateFileProperty(_fileGenerator, RepositoryPath(), RepositoryName());
    }
}