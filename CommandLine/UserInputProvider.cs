using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;

namespace Messerli.CommandLine
{
    public class UserInputProvider : IUserInputProvider
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IConsoleWriter _consoleWriter;
        private readonly Dictionary<string, UserInputDescription> _knownUserInputs = new Dictionary<string, UserInputDescription>();

        public UserInputProvider(IConsoleReader consoleReader, IConsoleWriter consoleWriter)
        {
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        public void RegisterVariable(UserInputDescription description)
        {
            _knownUserInputs.Add(description.VariableName, description);
        }

        public void AskUser()
        {
            foreach (var variable in _knownUserInputs.Select(v => v.Value).Where(v => v.IsNeeded.Value))
            {
                _consoleWriter.WriteLine($"I need input for the variable '{variable.VariableName}'");
                variable.Value = Option.Some(_consoleReader.ReadLine());
            }
        }

        public Dictionary<string, string> View()
        {
            return _knownUserInputs
                .Select(kv => kv.Value)
                .ToDictionary(v => v.VariableName, v => v.Value.OrElse("BAD VALUE!"));
        }

        public string Value(string variableName)
        {
            var x = _knownUserInputs.TryGetValue(key: variableName);

            return x.Match(
                none: () => throw new Exception($"Variable '{variableName}' is not a registered user input."),
                some: userInput => userInput.Value).OrElse("BAD");
        }
    }
}