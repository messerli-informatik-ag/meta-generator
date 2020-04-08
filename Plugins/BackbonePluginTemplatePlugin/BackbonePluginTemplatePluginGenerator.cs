using System;
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
        private readonly IGlobalJsonManipulator _globalJsonManipulator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IDotnetToolInstaller _dotnetToolInstaller;

        private static readonly MsBuildSdk BackbonePluginSdk = new MsBuildSdk("Messerli.Backbone.PluginSdk", "0.3.0");
        private static readonly MsBuildSdk CentralPackageVersionsSdk = new MsBuildSdk("Microsoft.Build.CentralPackageVersions", "2.0.52");
        private static readonly NugetPackageSource InternalNugetServer = new NugetPackageSource("Internal Nuget Server", "https://nuget.messerli.ch/v3/index.json");

        public BackbonePluginTemplatePluginGenerator(
            IConsoleWriter consoleWriter,
            IFileGenerator fileGenerator,
            IFileManipulator fileManipulator,
            INugetConfigurationManipulator nugetConfigurationManipulator,
            INugetPackageSourceManipulator nugetPackageSourceManipulator,
            IGlobalJsonManipulator globalJsonManipulator,
            IUserInputProvider userInputProvider,
            IDotnetToolInstaller dotnetToolInstaller)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _fileManipulator = fileManipulator;
            _nugetConfigurationManipulator = nugetConfigurationManipulator;
            _nugetPackageSourceManipulator = nugetPackageSourceManipulator;
            _globalJsonManipulator = globalJsonManipulator;
            _userInputProvider = userInputProvider;
            _dotnetToolInstaller = dotnetToolInstaller;
        }

        public string Description => "Create a new Backbone Plugin";

        public string Name => "backbone-plugin-template-plugin";

        private string PluginName => _userInputProvider.Value(VariableConstant.PluginName);

        private VariantType PluginVariant => ParsePluginVariant(_userInputProvider.Value(VariableConstant.PluginVariant));

        private string SolutionDirectory => _userInputProvider.Value(VariableConstant.SolutionDirectory);

        private bool UsesCentralPackageVersionsSdk => ParseUsesCentralPackageVersions(_userInputProvider.Value(VariableConstant.UsesCentralPackageVersions));

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
            var msbuildSdkTask = AddMsBuildSdkToGlobalJson();
            var tasks = new[]
            {
                templateFileCreationTask,
                solutionModificationTask,
                nugetConfigTask,
                msbuildSdkTask,
            };

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
            const string backboneToolName = "backbone";
            _dotnetToolInstaller.InstallTool(SolutionDirectory, backboneToolName);
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
            const string nugetConfigFileName = "NuGet.config";
            var nugetConfigFilePath = Path.Combine(SolutionDirectory, nugetConfigFileName);

            var nugetConfigModification = CreateInternalNugetServerModification();
            return _nugetConfigurationManipulator.ModifyNugetConfiguration(nugetConfigFilePath, nugetConfigModification);
        }

        private NugetConfigurationModification CreateInternalNugetServerModification()
            => new NugetConfigurationModificationBuilder(_nugetPackageSourceManipulator)
                .AddPackageSource(InternalNugetServer)
                .Build();

        private Task AddMsBuildSdkToGlobalJson()
        {
            const string globalJsonFileName = "global.json";
            var globalJsonFilePath = Path.Combine(SolutionDirectory, globalJsonFileName);

            var globalJsonModification = CreateMsBuildSdk();
            return _globalJsonManipulator.ModifyGlobalJson(globalJsonFilePath, globalJsonModification);
        }

        private GlobalJsonModification CreateMsBuildSdk()
        {
            var globalJsonModificationBuilder = new GlobalJsonModificationBuilder().AddMsBuildSdk(BackbonePluginSdk);
            if (UsesCentralPackageVersionsSdk)
            {
                globalJsonModificationBuilder = globalJsonModificationBuilder.AddMsBuildSdk(CentralPackageVersionsSdk);
            }

            return globalJsonModificationBuilder.Build();
        }

        private static VariantType ParsePluginVariant(string variantType)
            => (VariantType)int.Parse(variantType);

        private static bool ParseUsesCentralPackageVersions(string input)
            => bool.TryParse(input, out var addCentralPackageVersions)
                ? addCentralPackageVersions
                : throw new InvalidOperationException("Unable to convert user input CentralPackageVersions to bool");

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
