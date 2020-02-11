using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using Funcky.Extensions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class UserInputProvider : IUserInputProvider
    {
        private readonly ITemplateLoader _templateLoader;
        private readonly Dictionary<string, IUserInputDescription> _knownUserInputs = new Dictionary<string, IUserInputDescription>();
        private readonly Func<UserInputDescriptionBuilder> _newInputDescriptionBuilder;
        private readonly Func<VariableType, IVariableRequester> _variableRequesterFactory;
        private readonly DataContractJsonSerializer _jsonSerializer;

        public UserInputProvider(
            ITemplateLoader templateLoader,
            Func<UserInputDescriptionBuilder> newInputDescriptionBuilder,
            Func<VariableType, IVariableRequester> variableRequesterFactory)
        {
            _templateLoader = templateLoader;
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

        public void AskUser()
        {
            VerifyUserInputs();

            foreach (var variable in _knownUserInputs.Select(v => v.Value).Where(v => v.IsNeeded.Value))
            {
                var variableRequster = _variableRequesterFactory(variable.VariableType);

                variable.Value = variableRequster.RequestValue(variable);
            }
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
            var builder = _newInputDescriptionBuilder();

            RegisterVariableName(variable.Name, builder);
            RegisterVariableQuestion(variable.Question, builder);
            RegisterVariableType(variable.GetVariableType(), builder);
            RegisterSelectionValues(variable, builder);

            RegisterVariable(builder.Build());
        }

        private void RegisterSelectionValues(Variable variable, UserInputDescriptionBuilder builder)
        {
            if (variable.GetVariableType() == VariableType.Selection && variable.SelectionValues != null && variable.SelectionValues.Count > 0)
            {
                builder.SetSelectionValues(variable.SelectionValues);
            }
        }

        private void RegisterVariableQuestion(string? variableQuestion, UserInputDescriptionBuilder builder)
        {
            if (variableQuestion != null)
            {
                builder.SetVariableQuestion(variableQuestion);
            }
        }

        private void RegisterVariableType(VariableType variableType, UserInputDescriptionBuilder builder)
        {
            builder.SetVariableType(variableType);
        }

        private IUserInputDescription GetUserInputDescription(string variableName)
        {
            var x = _knownUserInputs
                .TryGetValue(key: variableName);

            return x.Match(
                none: () => NoValue(variableName),
                some: userInput => userInput);
        }

        private static IUserInputDescription NoValue(in string variableName)
        {
            throw new Exception($"No value known for '{variableName}'");
        }

        private static void RegisterVariableName(string? variableName, UserInputDescriptionBuilder builder)
        {
            if (variableName == null)
            {
                throw new Exception("A variable must have a name.");
            }

            builder.SetVariableName(variableName);
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