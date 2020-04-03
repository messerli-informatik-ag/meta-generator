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

        public string Description => "Create a new Backbone Plugin";

        public string Name => "backbone-plugin-template-plugin";

        private string PluginName => _userInputProvider.Value(VariableConstant.PluginName);

        private VariantType PluginVariant => ParsePluginVariant(_userInputProvider.Value(VariableConstant.PluginVariant));

        private string SolutionDirectory => _userInputProvider.Value(VariableConstant.SolutionDirectory);

        public void Register()
            => _userInputProvider.RegisterVariablesFromTemplate(Template.VariableDeclarations);

        public void Prepare()
        {
        }

        public void Generate()
        {
            _consoleWriter.WriteLine($"Creating Plugin: {PluginName}");
            var tasks = CreatePluginVariant();

            tasks.Add(_fileManipulator.AddProjectsToSolution(
                GetSolutionInfoBuilder().Build(),
                new[] { GetProjectInfoBuilder().Build(), GetProjectTestInfoBuilder().Build() }));

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
        }

        private static VariantType ParsePluginVariant(string variantType)
            => (VariantType)int.Parse(variantType);

        private string GetRepositoryPath()
            => Path.Combine(SolutionDirectory, PluginName);

        private string GetTestRepositoryPath()
            => Path.Combine(SolutionDirectory, GetTestProjectName());

        private string GetTestProjectName()
            => $"{PluginName}.{TestFolder}";

        private List<Task> CreatePluginVariant()
        {
            var templateFileProperty = CreateTemplateFileProperty();
            return PluginVariant switch
            {
                VariantType.MinimalPluginTemplate => new Variants.MinimalPlugin.PluginVariant(templateFileProperty).CreateTemplateFiles(),
                VariantType.PluginTemplate => new Variants.ViewPlugin.PluginVariant(templateFileProperty).CreateTemplateFiles(),
                VariantType.DatabaseAccessPluginTemplate => new[] { CreateTemplateFiles(templateFileProperty) }.ToList(),
                _ => throw new InvalidOperationException(),
            };
        }

        private static Task CreateTemplateFiles(TemplateFileProperty templateFileProperty)
        {
            var glob = $"templates/DatabaseAccessPluginTemplate/**/*";
            var destination = templateFileProperty.PluginPath;
            var templateNameValues = new Dictionary<string, string>
            {
                { "fileExtension", "cs" },
                { "templateFileExtension", "mustache" },
                { "pluginName", templateFileProperty.PluginName },
            };
            return templateFileProperty.FileGenerator.FromTemplateGlob(glob, destination, templateNameValues);
        }

        private SolutionInfo.Builder GetSolutionInfoBuilder()
            => new SolutionInfo.Builder()
                .WithPath(Directory.GetFiles(SolutionDirectory, "*.sln").FirstOrDefault());

        private ProjectInfo.Builder GetProjectInfoBuilder()
            => new ProjectInfo.Builder()
                .WithName(PluginName)
                .WithPath(Path.Combine(GetRepositoryPath(), $"{PluginName}.{ProjectFileExtension}"));

        private ProjectInfo.Builder GetProjectTestInfoBuilder()
            => new ProjectInfo.Builder()
                .WithName(GetTestProjectName())
                .WithPath(Path.Combine(GetTestRepositoryPath(), $"{PluginName}.{ProjectFileExtension}"));

        private TemplateFileProperty CreateTemplateFileProperty()
            => new TemplateFileProperty(_fileGenerator, SolutionDirectory, PluginName);
    }
}