using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator
{
    internal class PluginManager : IPluginManager
    {
        private readonly IConsoleWriter _consoleWriter;

        public PluginManager(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public int Install(string pluginName)
        {
            _consoleWriter.WriteLine($"{pluginName} installed");

            return ExitCode.Success;
        }

        public int Uninstall(string pluginName)
        {
            _consoleWriter.WriteLine($"{pluginName} uninstalled");

            return ExitCode.Success;
        }
    }
}