using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Drawing;
using System.Linq;
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

        public GenerationSteps(IUserInputProvider userInputProvider, IExecutingPluginAssemblyProvider assemblyProvider, ITools tools, ITimeKeeper timeKeeper, IConsoleWriter consoleWriter)
        {
            _userInputProvider = userInputProvider;
            _assemblyProvider = assemblyProvider;
            _tools = tools;
            _timeKeeper = timeKeeper;
            _consoleWriter = consoleWriter;
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
            catch (System.Exception exception)
            {
                _consoleWriter.WriteLine(exception.Message.Pastel(Color.OrangeRed));
                result = ExitCode.ExceptionOccured;
#if DEBUG
                if (exception.StackTrace != null)
                {
                    _consoleWriter.WriteLine(exception.StackTrace);
                }
#endif
            }

            return result;
        }

        private Dictionary<string, string> UserArguments(InvocationContext context)
        {
            Dictionary<string, string> userArguments = new Dictionary<string, string>();

            foreach (var variable in _userInputProvider.GetVariableValues())
            {
                if (context.ParseResult.ValueForOption(UserUptionFormat.ToUserOption(variable.Key)) is string userValue)
                {
                    userArguments.Add(variable.Key, userValue);
                }
            }

            return userArguments;
        }

        private bool VerifyTools()
        {
            var unavailableTools = _tools.VerifyTools().ToList();

            foreach (var (toolName, tool) in unavailableTools)
            {
                _consoleWriter.WriteLine($"Tool '{toolName}' is necessary for this plugin and has not been found on your machine.".Pastel(Color.OrangeRed));
            }

            return unavailableTools.Any() == false;
        }
    }
}
