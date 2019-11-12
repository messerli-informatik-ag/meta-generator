using System.Collections.Generic;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGeneratorPluginProjects
{
    public class ProjectGeneratorPluginProjectsGenerator : IProjectGenerator
    {
        private const string VariableDeclarations = "Messerli.ProjectGeneratorPluginProjects.templates.VariableDeclarations.json";
        private const string ProjectName = "ProjectName";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;

        public ProjectGeneratorPluginProjectsGenerator(IConsoleWriter consoleWriter, IFileGenerator fileGenerator, IUserInputProvider userInputProvider)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
        }

        public string Name => "Create a plugin for the project generator. (new project type)";

        public string ShortName => "project-generator.plugin";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);
        }

        public void Generate()
        {
            var projectName = _userInputProvider.Value(ProjectName);

            _consoleWriter.WriteLine($"Creating the plugin '{projectName}' for the project generator.");

            var tasks = new List<Task>
            {
            };

            Task.WaitAll(tasks.ToArray());
        }
    }
}