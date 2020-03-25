using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private const string RepositoryName = "RepositoryName";
        private const string PluginVariant = "PluginVariant";
        private const string SolutionPath = "TargetPath";

        private const string TestFolder = "Test";
        private const string ProjectFileExtension = "csproj";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IFileManipulator _fileManipulator;
        private readonly IUserInputProvider _userInputProvider;

        public BackbonePluginTemplatePluginGenerator(
            IConsoleWriter consoleWriter,
            IFileGenerator fileGenerator,
            IFileManipulator fileManipulator,
            IUserInputProvider userInputProvider)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _fileManipulator = fileManipulator;
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
            _consoleWriter.WriteLine($"Creating Plugin: {GetProjectName()}");
            var tasks = CreatePluginVariant(BackbonePluginVariant())
                .CreateTemplateFiles();

            tasks.Add(_fileManipulator.AddProjectsToSolution(
                GetSolutionInfoBuilder().Build(),
                new[] { GetProjectInfoBuilder().Build(), GetProjectTestInfoBuilder().Build() }));

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
        }

        private string GetProjectName()
            => _userInputProvider.Value(RepositoryName);

        private VariantType BackbonePluginVariant()
            => (VariantType)int.Parse(_userInputProvider.Value(PluginVariant));

        private string GetSolutionPath()
            => _userInputProvider.Value(SolutionPath);

        private string GetRepositoryPath()
            => Path.Combine(GetSolutionPath(), GetProjectName());

        private string GetTestRepositoryPath()
            => Path.Combine(GetSolutionPath(), GetTestProjectName());

        private string GetTestProjectName()
            => $"{GetProjectName()}.{TestFolder}";

        private IPluginVariant CreatePluginVariant(VariantType variant)
            => variant switch
            {
                VariantType.MinimalPluginTemplate => new Variants.MinimalPluginTemplate.PluginVariant(CreateTemplateFileProperty()),
                VariantType.PluginTemplate => new Variants.PluginTemplate.PluginVariant(CreateTemplateFileProperty()),
                VariantType.DatabaseAccessPluginTemplate => new Variants.DatabaseAccessPluginTemplate.PluginVariant(CreateTemplateFileProperty()),
                _ => throw new InvalidOperationException(),
            };

        private SolutionInfo.Builder GetSolutionInfoBuilder()
            => new SolutionInfo.Builder()
                .WithPath(Directory.GetFiles(GetSolutionPath(), "*.sln").FirstOrDefault());

        private ProjectInfo.Builder GetProjectInfoBuilder()
            => new ProjectInfo.Builder()
                .WithName(GetProjectName())
                .WithPath(Path.Combine(GetRepositoryPath(), $"{GetProjectName()}.{ProjectFileExtension}"));

        private ProjectInfo.Builder GetProjectTestInfoBuilder()
            => new ProjectInfo.Builder()
                .WithName(GetTestProjectName())
                .WithPath(Path.Combine(GetTestRepositoryPath(), $"{GetProjectName()}.{ProjectFileExtension}"));

        private TemplateFileProperty CreateTemplateFileProperty()
            => new TemplateFileProperty(_fileGenerator, GetSolutionPath(), GetProjectName());
    }
}