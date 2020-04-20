using System;
using System.Drawing;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Pastel;

namespace Messerli.MetaGenerator
{
    internal class ExceptionFormatter : IExceptionFormatter
    {
        private readonly IConsoleWriter _consoleWriter;
        private readonly IGlobalOptions _globalOptions;
        private readonly IExecutingPluginAssemblyProvider _executingPluginAssemblyProvider;

        public ExceptionFormatter(IConsoleWriter consoleWriter, IGlobalOptions globalOptions, IExecutingPluginAssemblyProvider executingPluginAssemblyProvider)
        {
            _consoleWriter = consoleWriter;
            _globalOptions = globalOptions;
            _executingPluginAssemblyProvider = executingPluginAssemblyProvider;
        }

        public void FormatException(Exception exception)
        {
            _consoleWriter.WriteLine($"Exception in module: {Module()}");
            _consoleWriter.WriteLine(exception.Message.Pastel(Color.OrangeRed));

            if (_globalOptions.Verbose && exception.StackTrace is { })
            {
                _consoleWriter.WriteLine(exception.StackTrace);
            }
        }

        private string Module()
        {
            return _executingPluginAssemblyProvider.HasPluginContext
                ? _executingPluginAssemblyProvider.PluginAssembly.GetName().Name!
                : "MetaGenerator";
        }
    }
}
