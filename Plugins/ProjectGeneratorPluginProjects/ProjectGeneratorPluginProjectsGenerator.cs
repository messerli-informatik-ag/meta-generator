using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGeneratorPluginProjects
{
    public class ProjectGeneratorPluginProjectsGenerator : IProjectGenerator
    {
        private const string VariableDeclarations = "Messerli.ProjectGeneratorPluginProjects.templates.VariableDeclarations.json";
        private const string VariableDeclarationsTemplate = "Messerli.ProjectGeneratorPluginProjects.templates.VariableDeclarations.json.template";
        private const string PluginProjectFileTemplate = "Messerli.ProjectGeneratorPluginProjects.templates.plugin.csproj.template";
        private const string GeneratorFileTemplate = "Messerli.ProjectGeneratorPluginProjects.templates.generator.source.template";
        private const string ModuleFileTemplate = "Messerli.ProjectGeneratorPluginProjects.templates.module.source.template";
        private const string PaketReferencesTemplate = "Messerli.ProjectGeneratorPluginProjects.templates.paket.template";
        private const string PublishScript = "Messerli.ProjectGeneratorPluginProjects.templates.publish.template";

        private const string ProjectName = "ProjectName";
        private const string ProjectDescription = "ProjectDescription";
        private const string ProjectGeneratorPath = "ProjectGeneratorPath";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IFileManipulator _fileManipulator;
        private readonly IUserInputProvider _userInputProvider;

        public ProjectGeneratorPluginProjectsGenerator(
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

        public string Name => "Create a plugin for the project generator. (new project type)";

        public string ShortName => "project-generator.plugin";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);

            _userInputProvider.AddValidation(ProjectName, Validations.ValuePresent);
            _userInputProvider.AddValidation(ProjectName, Validations.NoWhiteSpace);
            _userInputProvider.AddValidation(ProjectGeneratorPath, new SimpleValidation(path => File.Exists(Path.Combine(path, "ProjectGenerator.sln")), "No ProjectGenerator.sln found at location."));
            _userInputProvider.AddValidation(ProjectDescription, Validations.ValuePresent);
        }

        public void Prepare()
        {
        }

        public void Generate()
        {
            _consoleWriter.WriteLine($"Creating the plugin '{_userInputProvider.Value(ProjectName)}' for the project generator.");

            var tasks = new List<Task>
            {
                _fileGenerator.FromTemplate(PluginProjectFileTemplate, Path.Combine(GetPluginPath(), $"{_userInputProvider.Value(ProjectName)}.csproj"), Encoding.UTF8),
                _fileGenerator.FromTemplate(GeneratorFileTemplate, Path.Combine(GetPluginPath(), $"{_userInputProvider.Value(ProjectName)}Generator.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(ModuleFileTemplate, Path.Combine(GetPluginPath(), $"{_userInputProvider.Value(ProjectName)}Module.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(VariableDeclarationsTemplate, Path.Combine(GetPluginPath(), "templates", "VariableDeclarations.json"), new UTF8Encoding(false)),
                _fileGenerator.FromTemplate(PaketReferencesTemplate, Path.Combine(GetPluginPath(), "paket.references"), Encoding.UTF8),
                _fileManipulator.AppendTemplate(PublishScript, Path.Combine(GetSolutionPath(), "publish.ps1")),
                _fileManipulator.AddProjectToSolution("Plugins", ProjectName, Path.Combine(GetPluginPath(), $"{_userInputProvider.Value(ProjectName)}.csproj"), Path.Combine(GetSolutionPath(), "ProjectGenerator.sln")),
            };

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
            using (var repo = new Repository(GetSolutionPath()))
            {
                Commands.Stage(repo, Path.Combine("Plugins", _userInputProvider.Value(ProjectName), "*"));
            }
        }

        private string GetSolutionPath()
        {
            return _userInputProvider.Value(ProjectGeneratorPath);
        }

        private string GetPluginPath()
        {
            return Path.Combine(GetSolutionPath(), "Plugins", _userInputProvider.Value(ProjectName));
        }
    }
}