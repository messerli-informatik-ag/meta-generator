﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.NonInteractivePlugin
{
    public class NonInteractivePluginGenerator : IMetaGenerator
    {
        private const string VariableDeclarations = "Messerli.NonInteractivePlugin.templates.VariableDeclarations.json";
        private const string GeneratorName = "GeneratorName";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IVariableProvider _variableProvider;

        public NonInteractivePluginGenerator(IConsoleWriter consoleWriter, IFileGenerator fileGenerator, IUserInputProvider userInputProvider, IVariableProvider variableProvider)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
            _variableProvider = variableProvider;
        }

        public string Name => "Coole description";

        public string ShortName => "give.me.a.name";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);
        }

        public void Prepare()
        {
            // for example tfs checkout
        }

        public void Generate()
        {
            var generatorName = _userInputProvider.Value(GeneratorName);

            _consoleWriter.WriteLine($"Creating ... '{generatorName}' ...");

            var tasks = new List<Task>
            {
            };

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
            // for example git add
        }
    }
}