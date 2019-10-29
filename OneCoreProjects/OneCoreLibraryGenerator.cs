using System;
using System.IO;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;
using static Messerli.CommandLineAbstractions.UserInputDescription;

namespace Messerli.OneCoreProjects
{
    public class OneCoreLibraryGenerator : IProjectGenerator
    {
        private const string SharedTargetTemplate = "Messerli.OneCoreProjects.templates.Shared.targets.template";
        private const string ProjectTemplate = "Messerli.OneCoreProjects.templates.project.csproj.template";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly Func<UserInputDescriptionBuilder> _newInputDescriptionBuilder;

        public OneCoreLibraryGenerator(IConsoleWriter consoleWriter, IFileGenerator fileGenerator, IUserInputProvider userInputProvider, Func<UserInputDescriptionBuilder> newInputDescriptionBuilder)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
            _newInputDescriptionBuilder = newInputDescriptionBuilder;
        }

        public string Name => "Create One.Core library project";

        public string ShortName => "one.core.library";

        public void Register()
        {
            var builder = _newInputDescriptionBuilder();

            builder.SetVariableName("ProjectName");

            _userInputProvider.RegisterVariable(builder.Build());
        }

        public void Generate()
        {
            var projectName = "SomeProject";

            _consoleWriter.WriteLine("Generate a C# library for the One.Core project");

            _consoleWriter.WriteLine($"ProjectName: {_userInputProvider.Value("ProjectName")}");

            _fileGenerator.FromTemplate(SharedTargetTemplate, Path.Combine("Config", "Shared.targets"));
            _fileGenerator.FromTemplate(ProjectTemplate, Path.Combine(projectName, $"{projectName}.csproj"));
        }
    }
}