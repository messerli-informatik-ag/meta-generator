using System.Collections.Generic;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.NativeProjectsPlugin
{
    internal class NativeProjectsPluginGenerator : IMetaGenerator
    {
        private const string VariableDeclarations = "Messerli.NativeProjectsPlugin.templates.VariableDeclarations.json";
        private const string ProjectName = "ProjectName";
        private const string Branch = "Branch";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IVariableProvider _variableProvider;
        private readonly ITfsPaths _tfsPaths;

        public NativeProjectsPluginGenerator(IConsoleWriter consoleWriter, IFileGenerator fileGenerator, IUserInputProvider userInputProvider, IVariableProvider variableProvider, ITfsPaths tfsPaths)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
            _variableProvider = variableProvider;
            _tfsPaths = tfsPaths;
        }

        public string Name => "This plugin creates native C++ projects for the All Projects solution.";

        public string ShortName => "rmiprod.native";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);

            _userInputProvider[Branch].VariableSelectionValues.AddRange(_tfsPaths.Branches());
        }

        public void Prepare()
        {
            // for example tfs checkout
        }

        public void Generate()
        {
            var generatorName = _userInputProvider.Value(ProjectName);

            _consoleWriter.WriteLine($"Creating ... '{generatorName}' ...");

            var tasks = new List<Task>
            {
            };

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
            // for example git add
        }
    }
}