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
            if (exception is AggregateException aggregateException)
            {
                FlattenException(aggregateException);
            }
            else
            {
                FormatSingleException(exception);
            }
        }

        private void FormatSingleException(Exception exception)
        {
            _consoleWriter.WriteLine($"Exception in module: {Module()}".Pastel(Color.LightGoldenrodYellow));
            _consoleWriter.WriteLine(exception.Message.Pastel(Color.OrangeRed));

            if (_globalOptions.Verbose && exception.StackTrace is { } stackTrace)
            {
                _consoleWriter.WriteLine(stackTrace);
            }
        }

        private void FlattenException(AggregateException aggregateException)
        {
            foreach (var innerException in aggregateException.InnerExceptions)
            {
                FormatException(innerException);
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
