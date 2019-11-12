using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.OneCoreProjects
{
    public class OneCoreLibraryGenerator : IProjectGenerator
    {
        private const string VariableDeclarations = "Messerli.OneCoreProjects.templates.VariableDeclarations.json";
        private const string SharedTargetTemplate = "Messerli.OneCoreProjects.templates.Shared.targets.template";
        private const string ProjectTemplate = "Messerli.OneCoreProjects.templates.project.csproj.template";
        private const string ProjectName = "ProjectName";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;

        public OneCoreLibraryGenerator(IConsoleWriter consoleWriter, IFileGenerator fileGenerator, IUserInputProvider userInputProvider)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
        }

        public string Name => "Create One.Core library project";

        public string ShortName => "one.core.library";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);
        }

        public void Generate()
        {
            _consoleWriter.WriteLine("Generate a C# library for the One.Core project");

            var projectName = _userInputProvider.Value(ProjectName);

            var tasks = new List<Task>
            {
                _fileGenerator.FromTemplate(SharedTargetTemplate, Path.Combine("Config", "Shared.targets")),
                _fileGenerator.FromTemplate(ProjectTemplate, Path.Combine(projectName, $"{projectName}.csproj")),
            };

            Task.WaitAll(tasks.ToArray());
        }
    }
}