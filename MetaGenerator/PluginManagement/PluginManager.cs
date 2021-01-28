using System;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator.PluginManagement
{
    internal class PluginManager : IPluginManager
    {
        private readonly IConsoleWriter _consoleWriter;
        private readonly IExceptionFormatter _exceptionFormatter;
        private readonly IPluginRepository _pluginRepository;

        public PluginManager(IConsoleWriter consoleWriter, IExceptionFormatter exceptionFormatter, IPluginRepository pluginRepository)
        {
            _consoleWriter = consoleWriter;
            _exceptionFormatter = exceptionFormatter;
            _pluginRepository = pluginRepository;
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

        public async Task<int> Search()
        {
            try
            {
                _consoleWriter.WriteLine($"Plugins found in {_pluginRepository.Name}");
                foreach (var pluginName in await _pluginRepository.Plugins())
                {
                    _consoleWriter.WriteLine($"* {pluginName}");
                }

                return ExitCode.Success;
            }
            catch (Exception exception)
            {
                _exceptionFormatter.FormatException(exception);
                return ExitCode.ExceptionOccured;
            }
        }
    }
}
