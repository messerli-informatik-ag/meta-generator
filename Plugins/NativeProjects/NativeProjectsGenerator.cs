using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.NativeProjects
{
    public class NativeProjectsGenerator : IProjectGenerator
    {
        private const string VariableDeclarations = "Messerli.NativeProjects.templates.VariableDeclarations.json";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;

        public NativeProjectsGenerator(IConsoleWriter consoleWriter, IFileGenerator fileGenerator, IUserInputProvider userInputProvider)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
        }

        public string Name => "C++ Testproject";

        public string ShortName => "native.test";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);
        }

        public void Generate()
        {
            _consoleWriter.WriteLine("Generate a native (C++) test project");
        }

        public void PostGenerate()
        {
        }
    }
}