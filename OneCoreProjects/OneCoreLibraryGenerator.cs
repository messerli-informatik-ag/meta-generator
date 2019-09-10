using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;

namespace Messerli.OneCoreProjects
{
    public class OneCoreLibraryGenerator : IProjectGenerator
    {
        private const string SharedTargetTemplate = "Messerli.OneCoreProjects.templates.Shared.targets.template";
        private const string ProjectTemplate = "Messerli.OneCoreProjects.templates.project.csproj.template";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;

        public OneCoreLibraryGenerator(IConsoleWriter consoleWriter, IFileGenerator fileGenerator)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
        }

        public string Name => "Create One.Core library project";

        public string ShortName => "one.core.library";

        public void Generate()
        {
            var projectName = "SomeProject";

            _consoleWriter.WriteLine("Generate a C# library for the One.Core project");

            _fileGenerator.FromTemplate(SharedTargetTemplate, @"Config/Shared.targets");
            _fileGenerator.FromTemplate(ProjectTemplate, $"{projectName}/{projectName}.csproj");
        }
    }
}