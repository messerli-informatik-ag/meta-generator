using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;

namespace Messerli.OneCoreProjects
{
    public class OneCoreLibraryGenerator : IProjectGenerator
    {
        private readonly IConsoleWriter _consoleWriter;

        public OneCoreLibraryGenerator(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public string Name => "Create One.Core library project";

        public string ShortName => "one.core.library";

        public void Generate()
        {
            _consoleWriter.WriteLine("Generate a C# library for the One.Core project");
        }
    }
}