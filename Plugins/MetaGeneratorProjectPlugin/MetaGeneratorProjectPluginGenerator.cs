using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Funcky.Extensions;
using LibGit2Sharp;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Messerli.VsSolution.Model;
using Soltys.ChangeCase;

namespace Messerli.MetaGeneratorProjectPlugin
{
    internal class MetaGeneratorProjectPluginGenerator : IMetaGenerator
    {
        private const string VariableDeclarations = "Messerli.MetaGeneratorProjectPlugin.templates.VariableDeclarations.json";
        private const string VariableDeclarationsTemplate = "Messerli.MetaGeneratorProjectPlugin.templates.VariableDeclarations.json.template";
        private const string PluginProjectFileTemplate = "Messerli.MetaGeneratorProjectPlugin.templates.plugin.csproj.template";
        private const string GeneratorFileTemplate = "Messerli.MetaGeneratorProjectPlugin.templates.generator.source.template";
        private const string ModuleFileTemplate = "Messerli.MetaGeneratorProjectPlugin.templates.module.source.template";
        private const string PublishScript = "Messerli.MetaGeneratorProjectPlugin.templates.publish.template";

        private const string GeneratorName = "GeneratorName";
        private const string KebabGeneratorName = "KebabGeneratorName";
        private const string GeneratorPath = "GeneratorPath";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IFileManipulator _fileManipulator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IVariableProvider _variableProvider;

        public MetaGeneratorProjectPluginGenerator(
            IConsoleWriter consoleWriter,
            IFileGenerator fileGenerator,
            IFileManipulator fileManipulator,
            IUserInputProvider userInputProvider,
            IVariableProvider variableProvider)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _fileManipulator = fileManipulator;
            _userInputProvider = userInputProvider;
            _variableProvider = variableProvider;
        }

        public string Description => "Create a new plugin for this generator.";

        public string Name => "meta-generator-plugin";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);
        }

        public void Prepare()
        {
            _variableProvider
                .GetVariableValues()
                .TryGetValue(key: GeneratorName)
                .AndThen(name => _variableProvider.RegisterValue(KebabGeneratorName, name.ParamCase()));
        }

        public void Generate()
        {
            _consoleWriter.WriteLine($"Creating the plugin '{_userInputProvider.Value(GeneratorName)}' for the project generator.");

            var tasks = new List<Task>
            {
                _fileGenerator.FromTemplate(PluginProjectFileTemplate, Path.Combine(GetPluginPath(), $"{_userInputProvider.Value(GeneratorName)}.csproj"), Encoding.UTF8),
                _fileGenerator.FromTemplate(GeneratorFileTemplate, Path.Combine(GetPluginPath(), $"{_userInputProvider.Value(GeneratorName)}Generator.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(ModuleFileTemplate, Path.Combine(GetPluginPath(), $"{_userInputProvider.Value(GeneratorName)}Module.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(VariableDeclarationsTemplate, Path.Combine(GetPluginPath(), "templates", "VariableDeclarations.json"), new UTF8Encoding(false)),

                _fileManipulator.AppendTemplate(PublishScript, Path.Combine(GetSolutionPath(), "publish.ps1")),
                _fileManipulator.AddProjectsToSolution(GetSolutionInfoBuilder().Build(), GetProjectInfoBuilder().Build().Yield()),
            };

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
            using var repo = new Repository(GetSolutionPath());

            Commands.Stage(repo, Path.Combine("Plugins", _userInputProvider.Value(GeneratorName), "*"));
        }

        private SolutionInfo.Builder GetSolutionInfoBuilder()
        {
            return new SolutionInfo.Builder()
                .WithPath(Path.Combine(GetSolutionPath(), "MetaGenerator.sln"))
                .WithFilterFolder("Plugins");
        }

        private ProjectInfo.Builder GetProjectInfoBuilder()
        {
            return new ProjectInfo.Builder()
                .WithName(_userInputProvider.Value(GeneratorName))
                .WithPath(Path.Combine(GetPluginPath(), $"{_userInputProvider.Value(GeneratorName)}.csproj"));
        }

        private string GetSolutionPath()
        {
            return _userInputProvider.Value(GeneratorPath);
        }

        private string GetPluginPath()
        {
            return Path.Combine(GetSolutionPath(), "Plugins", _userInputProvider.Value(GeneratorName));
        }
    }
}