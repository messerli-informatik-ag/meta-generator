using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Drawing;
using System.Linq;
using Funcky.Extensions;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Messerli.ToolLoaderAbstractions;
using Pastel;

namespace Messerli.MetaGenerator
{
    internal class GenerationSteps : IGenerationSteps
    {
        private readonly IUserInputProvider _userInputProvider;
        private readonly IExecutingPluginAssemblyProvider _assemblyProvider;
        private readonly ITools _tools;
        private readonly ITimeKeeper _timeKeeper;
        private readonly IConsoleWriter _consoleWriter;
        private readonly IExceptionFormatter _exceptionFormatter;

        public GenerationSteps(IUserInputProvider userInputProvider, IExecutingPluginAssemblyProvider assemblyProvider, ITools tools, ITimeKeeper timeKeeper, IConsoleWriter consoleWriter, IExceptionFormatter exceptionFormatter)
        {
            _userInputProvider = userInputProvider;
            _assemblyProvider = assemblyProvider;
            _tools = tools;
            _timeKeeper = timeKeeper;
            _consoleWriter = consoleWriter;
            _exceptionFormatter = exceptionFormatter;
        }

        public int Execute(IMetaGenerator metaTypeGenerator, InvocationContext context)
        {
            var result = ExitCode.Success;

            _assemblyProvider.PluginAssembly = metaTypeGenerator.GetType().Assembly;

            try
            {
                _timeKeeper.MeasureTime(metaTypeGenerator.Register, "Registration");

                if (VerifyTools())
                {
                    _userInputProvider.AskUser(UserArguments(context));

                    _timeKeeper.MeasureTime(metaTypeGenerator.Prepare, "Prepartion");
                    _timeKeeper.MeasureTime(metaTypeGenerator.Generate, "Generation");
                    _timeKeeper.MeasureTime(metaTypeGenerator.TearDown, "Tear down");
                }
                else
                {
                    result = ExitCode.ToolMissing;
                }

                _timeKeeper.Print();
            }
            catch (Exception exception)
            {
                _exceptionFormatter.FormatException(exception);
                result = ExitCode.ExceptionOccured;
            }

            return result;
        }

        private Dictionary<string, string> UserArguments(InvocationContext context)
        {
            Dictionary<string, string> userArguments = new();

            foreach (var variable in _userInputProvider.GetVariableValues())
            {
                if (context.ParseResult.ValueForOption(UserOptionFormat.ToUserOption(variable.Key)) is string userValue)
                {
                    userArguments.Add(variable.Key, userValue);
                }
            }

            return userArguments;
        }

        private bool VerifyTools()
            => _tools
                .VerifyTools()
                .Select(ToolName)
                .Inspect(ToolIsNecessaryMessage)
                .None();

        private static string ToolName(KeyValuePair<string, ITool> tool)
            => tool.Key;

        private void ToolIsNecessaryMessage(string toolName)
            => _consoleWriter
                .WriteLine($"The Tool '{toolName}' is necessary for this plugin and has not been found on your machine.".Pastel(Color.OrangeRed));
    }
}
