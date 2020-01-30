using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGeneratorProjectPlugin
{
    internal class MetaGeneratorProjectPluginGenerator : IMetaGenerator
    {
        private const string VariableDeclarations = "Messerli.MetaGeneratorProjectPlugin.templates.VariableDeclarations.json";
        private const string VariableDeclarationsTemplate = "Messerli.MetaGeneratorProjectPlugin.templates.VariableDeclarations.json.template";
        private const string PluginProjectFileTemplate = "Messerli.MetaGeneratorProjectPlugin.templates.plugin.csproj.template";
        private const string GeneratorFileTemplate = "Messerli.MetaGeneratorProjectPlugin.templates.generator.source.template";
        private const string ModuleFileTemplate = "Messerli.MetaGeneratorProjectPlugin.templates.module.source.template";
        private const string PaketReferencesTemplate = "Messerli.MetaGeneratorProjectPlugin.templates.paket.template";
        private const string PublishScript = "Messerli.MetaGeneratorProjectPlugin.templates.publish.template";

        private const string GeneratorName = "GeneratorName";
        private const string GeneratorDescription = "GeneratorDescription";
        private const string GeneratorPath = "GeneratorPath";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IFileManipulator _fileManipulator;
        private readonly IUserInputProvider _userInputProvider;

        public MetaGeneratorProjectPluginGenerator(
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

        public string Name => "Create a new plugin for this generator.";

        public string ShortName => "meta-generator.plugin";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);

            _userInputProvider[GeneratorName].Validations.Add(Validations.ValuePresent);
            _userInputProvider[GeneratorName].Validations.Add(Validations.NoWhiteSpace);
            _userInputProvider[GeneratorPath].Validations.Add(new SimpleValidation(path => File.Exists(Path.Combine(path, "MetaGenerator.sln")), "No ProjectGenerator.sln found at location."));
            _userInputProvider[GeneratorDescription].Validations.Add(Validations.ValuePresent);
        }

        public void Prepare()
        {
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
                _fileGenerator.FromTemplate(PaketReferencesTemplate, Path.Combine(GetPluginPath(), "paket.references"), Encoding.UTF8),
                _fileManipulator.AppendTemplate(PublishScript, Path.Combine(GetSolutionPath(), "publish.ps1")),
                _fileManipulator.AddProjectToSolution("Plugins", _userInputProvider.Value(GeneratorName), Path.Combine(GetPluginPath(), $"{_userInputProvider.Value(GeneratorName)}.csproj"), Path.Combine(GetSolutionPath(), "MetaGenerator.sln"), null),
            };

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
            using var repo = new Repository(GetSolutionPath());

            Commands.Stage(repo, Path.Combine("Plugins", _userInputProvider.Value(GeneratorName), "*"));
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