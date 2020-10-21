using System.Collections.Generic;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.CppPropsReferencePlugin
{
    public class CppPropsReferencePluginGenerator : IMetaGenerator
    {
        private const string GeneratorName = "Cpp props reference plugin generator";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IVariableProvider _variableProvider;

        public CppPropsReferencePluginGenerator(IConsoleWriter consoleWriter, IFileGenerator fileGenerator, IUserInputProvider userInputProvider, IVariableProvider variableProvider)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
            _variableProvider = variableProvider;
        }

        public string Description => "Adds C++ project references to properties";

        public string Name => "cpp-props-reference-plugin";

        private string TargetProjectPath => _userInputProvider.Value(VariableConstant.TargetProjectPath);

        private string ReferencedProjectPath => _userInputProvider.Value(VariableConstant.ReferencedProjectPath);

        private string ApiMakroName => _userInputProvider.Value(VariableConstant.ApiMakroName);

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(Template.VariableDeclarations);
        }

        public void Prepare()
        {
            // for example tfs checkout
        }

        public void Generate()
        {
            var generatorName = _userInputProvider.Value(GeneratorName);

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
