using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.FileManipulatorAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.BackbonePluginTemplatePlugin
{
    public class BackbonePluginTemplatePluginGenerator : IMetaGenerator
    {
        private const string TestDirectorySuffix = "Test";
        private const string ProjectFileExtension = "csproj";
        private const string SolutionFileExtension = "sln";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IFileManipulator _fileManipulator;
        private readonly INugetConfigurationManipulator _nugetConfigurationManipulator;
        private readonly INugetPackageSourceManipulator _nugetPackageSourceManipulator;
        private readonly IUserInputProvider _userInputProvider;

        public BackbonePluginTemplatePluginGenerator(
            IConsoleWriter consoleWriter,
            IFileGenerator fileGenerator,
            IFileManipulator fileManipulator,
            INugetConfigurationManipulator nugetConfigurationManipulator,
            INugetPackageSourceManipulator nugetPackageSourceManipulator,
            IUserInputProvider userInputProvider)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _fileManipulator = fileManipulator;
            _nugetConfigurationManipulator = nugetConfigurationManipulator;
            _nugetPackageSourceManipulator = nugetPackageSourceManipulator;
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
            var templateFileCreationTask = CreateTemplateFilesForSelection();
            var solutionModificationTask = AddProjectsToSolution();
            var nugetConfigTask = AddInternalNugetServerToNugetConfig();
            var tasks = new[]
            {
                templateFileCreationTask,
                solutionModificationTask,
                nugetConfigTask,
            };

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
        }

        private Task AddProjectsToSolution()
        {
            var solutionInfo = GetSolutionInfo();
            var projectInfo = GetProjectInfo();
            var projectTestInfo = GetProjectTestInfo();
            return _fileManipulator.AddProjectsToSolution(
                solutionInfo,
                new[] { projectInfo, projectTestInfo });
        }

        private Task AddInternalNugetServerToNugetConfig()
        {
            var nugetConfigFileName = "NuGet.config";
            var nugetConfigFilePath = Path.Combine(SolutionDirectory, nugetConfigFileName);

            var nugetConfigModification = CreateInternalNugetServerModification();
            return _nugetConfigurationManipulator.ModifyNugetConfiguration(nugetConfigFilePath, nugetConfigModification);
        }

        private NugetConfigurationModification CreateInternalNugetServerModification()
            => new NugetConfigurationModificationBuilder(_nugetPackageSourceManipulator)
                .AddPackageSource(CreateInternalNugetServer())
                .Build();

        private NugetPackageSource CreateInternalNugetServer()
            => new NugetPackageSource("Internal Nuget Server", "https://nuget.messerli.ch/v3/index.json");

        private static VariantType ParsePluginVariant(string variantType)
            => (VariantType)int.Parse(variantType);

        private string GetProjectPath()
            => Path.Combine(SolutionDirectory, PluginName);

        private string GetTestProjectPath()
            => Path.Combine(SolutionDirectory, GetTestProjectName());

        private string GetTestProjectName()
            => $"{PluginName}.{TestDirectorySuffix}";

        private Task CreateTemplateFilesForSelection()
        {
            var templateName = PluginVariant.ToString();
            return CreateTemplateFiles(templateName);
        }

        private Task CreateTemplateFiles(string templateName)
        {
            var glob = $"templates/{templateName}/**/*";
            var templateNameValues = new Dictionary<string, string>
            {
                { "fileExtension", "cs" },
                { "templateFileExtension", "mustache" },
                { "pluginName", PluginName },
            };
            return _fileGenerator.FromTemplateGlob(glob, SolutionDirectory, templateNameValues);
        }

        private SolutionInfo GetSolutionInfo()
            => new SolutionInfo.Builder()
                .WithPath(Directory.GetFiles(SolutionDirectory, $"*.{SolutionFileExtension}").FirstOrDefault())
                .Build();

        private ProjectInfo GetProjectInfo()
        {
            var projectPath = GetProjectPath();
            var projectFileName = $"{PluginName}.{ProjectFileExtension}";
            var projectFilePath = Path.Combine(projectPath, projectFileName);
            return new ProjectInfo.Builder()
                .WithName(PluginName)
                .WithPath(projectFilePath)
                .Build();
        }

        private ProjectInfo GetProjectTestInfo()
        {
            var testProjectName = GetTestProjectName();
            var testProjectPath = GetTestProjectPath();
            var projectFileName = $"{testProjectName}.{ProjectFileExtension}";
            var testProjectFilePath = Path.Combine(testProjectPath, projectFileName);
            return new ProjectInfo.Builder()
                .WithName(GetTestProjectName())
                .WithPath(testProjectFilePath)
                .Build();
        }
    }
}
