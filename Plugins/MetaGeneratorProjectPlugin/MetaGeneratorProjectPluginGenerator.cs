using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Funcky.Extensions;
using LibGit2Sharp;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Soltys.ChangeCase;

namespace Messerli.MetaGeneratorProjectPlugin
{
    internal class MetaGeneratorProjectPluginGenerator : IMetaGenerator
    {
        private const string VariableDeclarations = "templates\\VariableDeclarations.json";

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
            var fileNameTemplateValues = new Dictionary<string, string>
            {
                { "fileExtension", "cs" },
                { "generatorName", _userInputProvider.Value(GeneratorName) },
            };

            var tasks = new[]
            {
                _fileGenerator.FromTemplateGlob("templates/**/*.mustache", GetPluginPath(), fileNameTemplateValues),
                _fileManipulator.AddProjectsToSolution(GetSolutionInfoBuilder().Build(), GetProjectInfoBuilder().Build().Yield()),
            };

            Task.WaitAll(tasks);
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