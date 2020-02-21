﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class UserInputProvider : IUserInputProvider
    {
        private readonly ITemplateLoader _templateLoader;
        private readonly IExecutingPluginAssemblyProvider _executingPluginAssemblyProvider;
        private readonly Dictionary<string, IUserInputDescription> _knownUserInputs = new Dictionary<string, IUserInputDescription>();
        private readonly Func<UserInputDescriptionBuilder> _newInputDescriptionBuilder;
        private readonly Func<VariableType, IVariableRequester> _variableRequesterFactory;
        private readonly DataContractJsonSerializer _jsonSerializer;

        public UserInputProvider(
            ITemplateLoader templateLoader,
            Func<UserInputDescriptionBuilder> newInputDescriptionBuilder,
            IExecutingPluginAssemblyProvider executingPluginAssemblyProvider,
            Func<VariableType, IVariableRequester> variableRequesterFactory)
        {
            _templateLoader = templateLoader;
            _executingPluginAssemblyProvider = executingPluginAssemblyProvider;
            _newInputDescriptionBuilder = newInputDescriptionBuilder;
            _variableRequesterFactory = variableRequesterFactory;
            _jsonSerializer = new DataContractJsonSerializer(typeof(List<Variable>));
        }

        public IUserInputDescription this[string key] => GetUserInputDescription(key);

        public void RegisterVariable(UserInputDescription description)
        {
            _knownUserInputs.Add(description.VariableName, description);
        }

        public void RegisterVariablesFromTemplate(string templateName)
        {
            foreach (var variable in GetVariablesFromTemplate(templateName))
            {
                RegisterVariablesFromJson(variable);
            }
        }

        public void AskUser(Dictionary<string, string> userArguments)
        {
            VerifyUserInputs();

            foreach (var variable in _knownUserInputs.Select(v => v.Value).Where(v => v.IsNeeded.Value))
            {
                var variableRequster = _variableRequesterFactory(variable.VariableType);

                variable.Value = Option.Some(variableRequster.RequestValue(variable, userArguments.TryGetValue(key: variable.VariableName)));
            }
        }

        public IEnumerable<IUserInputDescription> GetUserInputDescriptions()
        {
            return _knownUserInputs.Values;
        }

        public void Clear()
        {
            _knownUserInputs.Clear();
        }

        public Dictionary<string, string> GetVariableValues()
        {
            return _knownUserInputs
                .Select(kv => kv.Value)
                .ToDictionary(v => v.VariableName, v => v.Value.OrElse("BAD VALUE!"));
        }

        public string Value(string variableName)
        {
            return _knownUserInputs
                .TryGetValue(key: variableName)
                .Match(
                    none: () => throw new Exception($"Variable '{variableName}' is not a registered user input."),
                    some: userInput => userInput.Value).OrElse("should not happen");
        }

        private List<Variable> GetVariablesFromTemplate(string templateName)
        {
            using var stream = _templateLoader.GetTemplateStream(templateName);

            return (List<Variable>)_jsonSerializer
                .ReadObject(stream);
        }

        private void RegisterVariablesFromJson(Variable variable)
        {
            RegisterVariable(BuildUserInput(variable));
        }

        private UserInputDescription BuildUserInput(Variable variable)
        {
            return _newInputDescriptionBuilder()
                .RegisterVariableName(variable.Name)
                .RegisterVariableQuestion(variable.Question)
                .SetVariableType(variable.GetVariableType())
                .RegisterSelectionValues(variable)
                .RegisterVariableValidations(variable, _executingPluginAssemblyProvider.PluginAssembly)
                .Build();
        }

        private IUserInputDescription GetUserInputDescription(string variableName)
        {
            return _knownUserInputs
                .TryGetValue(key: variableName)
                .Match(
                    none: () => NoValue(variableName),
                    some: userInput => userInput);
        }

        private static IUserInputDescription NoValue(in string variableName)
        {
            throw new Exception($"No value known for '{variableName}'");
        }

        private void VerifyUserInputs()
        {
            foreach (var (key, variable) in _knownUserInputs)
            {
                VerifyVariable(variable);
            }
        }

        private static void VerifyVariable(IUserInputDescription variable)
        {
            if (variable.VariableType == VariableType.Selection && (variable.VariableSelectionValues == null || variable.VariableSelectionValues.Count == 0))
            {
                throw new Exception("If the variable type is selection, there must be at least one selection value.");
            }

            if (variable.VariableType != VariableType.Selection && variable.VariableSelectionValues != null && variable.VariableSelectionValues.Count > 0)
            {
                throw new Exception("You have specified values for a selection, but the type is not a selection.");
            }

            if (variable.VariableSelectionValues != null && variable.VariableSelectionValues.Any(selectionValue => string.IsNullOrEmpty(selectionValue.Value)))
            {
                throw new Exception("All selections value must have a valid string value!");
            }
        }
    }
}