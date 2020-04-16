using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.FileManipulatorAbstractions;
using Messerli.FileManipulatorAbstractions.Project;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.BackbonePluginTemplatePlugin
{
    public class BackbonePluginTemplatePluginGenerator : IMetaGenerator
    {
        private const string TestDirectorySuffix = "Test";
        private const string ProjectFileExtension = "csproj";
        private const string SolutionFileExtension = "sln";
        private const string CentralPackageVersionsSdkName = "Microsoft.Build.CentralPackageVersions";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IFileManipulator _fileManipulator;
        private readonly INugetConfigurationManipulator _nugetConfigurationManipulator;
        private readonly INugetPackageSourceManipulator _nugetPackageSourceManipulator;
        private readonly IGlobalJsonManipulator _globalJsonManipulator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IProjectManipulator _projectManipulator;
        private readonly IDotnetToolInstaller _dotnetToolInstaller;

        private static readonly MsBuildSdk BackbonePluginSdk = new MsBuildSdk("Messerli.Backbone.PluginSdk", "0.4.1");
        private static readonly MsBuildSdk CentralPackageVersionsSdk = new MsBuildSdk(CentralPackageVersionsSdkName, "2.0.52");
        private static readonly NugetPackageSource InternalNugetServer = new NugetPackageSource("Internal Nuget Server", "https://nuget.messerli.ch/v3/index.json");

        private static readonly PackageReference AutofacPackageReference =
            new PackageReferenceBuilder()
                .Name("Autofac")
                .Version("[5.1.2, 6)")
                .Build();

        private static readonly PackageReference FodyPackageReference =
            new PackageReferenceBuilder()
                .Name("Fody")
                .Version("[6.1.1, 7)")
                .PrivateAssets(new DependencyAssets.All())
                .Build();

        private static readonly PackageReference EqualsFobyPackageReference =
            new PackageReferenceBuilder()
                .Name("Equals.Fody")
                .Version("[4.0.1, 5)")
                .PrivateAssets(new DependencyAssets.All())
                .Build();

        private static readonly PackageReference MesserliBackbonePluginTestServerPackageReference =
            new PackageReferenceBuilder()
                .Name("Messerli.Backbone.PluginTestServer")
                .Version("[0.4.1, 0.5)")
                .Build();

        private static readonly PackageReference MesserliBackbonePluginTestUtilityPackageReference =
            new PackageReferenceBuilder()
                .Name("Messerli.Backbone.PluginTestUtility")
                .Version("[0.4.1, 0.5)")
                .Build();

        private static readonly PackageReference MesserliCodeStyle =
            new PackageReferenceBuilder()
                .Name("Messerli.CodeStyle")
                .Version("[1.1.0, 1.2)")
                .PrivateAssets(new DependencyAssets.All())
                .Build();

        private static readonly PackageReference XunitPackageReference =
            new PackageReferenceBuilder()
                .Name("xunit")
                .Version("[2.4.1, 3)")
                .Build();

        private static readonly PackageReference XunitrunnervisualstudioPackageReference =
            new PackageReferenceBuilder()
                .Name("xunit.runner.visualstudio")
                .Version("[2.4.1, 3)")
                .Build();

        public BackbonePluginTemplatePluginGenerator(
            IConsoleWriter consoleWriter,
            IFileGenerator fileGenerator,
            IFileManipulator fileManipulator,
            INugetConfigurationManipulator nugetConfigurationManipulator,
            INugetPackageSourceManipulator nugetPackageSourceManipulator,
            IGlobalJsonManipulator globalJsonManipulator,
            IUserInputProvider userInputProvider,
            IProjectManipulator projectManipulator,
            IDotnetToolInstaller dotnetToolInstaller)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _fileManipulator = fileManipulator;
            _nugetConfigurationManipulator = nugetConfigurationManipulator;
            _nugetPackageSourceManipulator = nugetPackageSourceManipulator;
            _globalJsonManipulator = globalJsonManipulator;
            _userInputProvider = userInputProvider;
            _projectManipulator = projectManipulator;
            _dotnetToolInstaller = dotnetToolInstaller;
        }

        public string Description => "Create a new Backbone Plugin";

        public string Name => "backbone-plugin-template-plugin";

        private string PluginName => _userInputProvider.Value(VariableConstant.PluginName);

        private VariantType PluginVariant => ParsePluginVariant(_userInputProvider.Value(VariableConstant.PluginVariant));

        private string SolutionDirectory => _userInputProvider.Value(VariableConstant.SolutionDirectory);

        private bool UsesCentralPackageVersionsSdk => ParseUsesCentralPackageVersions(_userInputProvider.Value(VariableConstant.UsesCentralPackageVersions));

        public void Register()
        {
            _dotnetToolInstaller.RegisterTool();
            _userInputProvider.RegisterVariablesFromTemplate(Template.VariableDeclarations);
        }

        public void Prepare()
        {
        }

        public void Generate()
        {
            _consoleWriter.WriteLine($"Creating Plugin: {PluginName}");

            GenerateAsync().Wait();
        }

        public async Task GenerateAsync()
        {
            await CreateTemplateFilesForSelection();
            await AddProjectsToSolution();
            await AddInternalNugetServerToNugetConfig();
            await AddMsBuildSdkToGlobalJson();
            await AddProjectReferences();
            await AddTestProjectReferences();
        }

        public void TearDown()
        {
            const string backboneToolName = "backbone";
            const string version = "0.5.1";
            _dotnetToolInstaller.InstallTool(SolutionDirectory, backboneToolName, version);
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

        private Task AddProjectReferences()
        {
            var modification = CreateProjectModification();
            return _projectManipulator.ManipulateProject(GetProjectFilePath(), modification);
        }

        private Task AddTestProjectReferences()
        {
            var modification = CreateTestProjectModification();
            return _projectManipulator.ManipulateProject(GetTestProjectFilePath(), modification);
        }

        private ProjectModification CreateProjectModification()
        {
            var projectModificationBuilder = new ProjectModificationBuilder();

            if (PluginVariant != VariantType.MinimalPlugin)
            {
                projectModificationBuilder = projectModificationBuilder.AddPackageReferences(CreateExtendedProjectPackageReferences());
            }

            if (UsesCentralPackageVersionsSdk)
            {
                projectModificationBuilder = projectModificationBuilder.AddSdk(CentralPackageVersionsSdkName);
            }

            return projectModificationBuilder.Build();
        }

        private ProjectModification CreateTestProjectModification()
        {
            var projectModificationBuilder = new ProjectModificationBuilder()
                .AddPackageReferences(CreateTestProjectPackageReferences());

            if (UsesCentralPackageVersionsSdk)
            {
                projectModificationBuilder = projectModificationBuilder.AddSdk(CentralPackageVersionsSdkName);
            }

            return projectModificationBuilder.Build();
        }

        private static IEnumerable<PackageReference> CreateExtendedProjectPackageReferences()
            => new[]
            {
                AutofacPackageReference,
                FodyPackageReference,
                EqualsFobyPackageReference,
                MesserliCodeStyle,
            };

        private static IEnumerable<PackageReference> CreateTestProjectPackageReferences()
            => new[]
            {
                MesserliBackbonePluginTestServerPackageReference,
                MesserliBackbonePluginTestUtilityPackageReference,
                MesserliCodeStyle,
                XunitPackageReference,
                XunitrunnervisualstudioPackageReference,
            };

        private static VariantType ParsePluginVariant(string variantType)
            => (VariantType)int.Parse(variantType);

        private static bool ParseUsesCentralPackageVersions(string input)
            => bool.TryParse(input, out var addCentralPackageVersions)
                ? addCentralPackageVersions
                : throw new InvalidOperationException("Unable to convert user input CentralPackageVersions to bool");

        private string GetProjectPath()
            => Path.Combine(SolutionDirectory, PluginName);

        private string GetProjectFilePath()
        {
            var projectPath = GetProjectPath();
            var projectFileName = $"{PluginName}.{ProjectFileExtension}";
            return Path.Combine(projectPath, projectFileName);
        }

        private string GetTestProjectPath()
            => Path.Combine(SolutionDirectory, GetTestProjectName());

        private string GetTestProjectName()
            => $"{PluginName}.{TestDirectorySuffix}";

        private string GetTestProjectFilePath()
        {
            var testProjectName = GetTestProjectName();
            var testProjectPath = GetTestProjectPath();
            var testProjectFileName = $"{testProjectName}.{ProjectFileExtension}";
            return Path.Combine(testProjectPath, testProjectFileName);
        }

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
            var projectFilePath = GetProjectFilePath();
            return new ProjectInfo.Builder()
                .WithName(PluginName)
                .WithPath(projectFilePath)
                .Build();
        }

        private ProjectInfo GetProjectTestInfo()
        {
            var testProjectFilePath = GetTestProjectFilePath();
            return new ProjectInfo.Builder()
                .WithName(GetTestProjectName())
                .WithPath(testProjectFilePath)
                .Build();
        }
    }
}
