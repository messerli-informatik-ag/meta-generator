using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;

namespace Messerli.NativeProjects
{
    public class NativeTestProjectGenerator : IProjectGenerator
    {
        private readonly IConsoleWriter _consoleWriter;

        public NativeTestProjectGenerator(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public string Name => "C++ Testproject";

        public string ShortName => "native.test";

        public void Register()
        {
            throw new System.NotImplementedException();
        }

        public void Generate()
        {
            _consoleWriter.WriteLine("Generate a native (C++) test project");
        }
    }
}