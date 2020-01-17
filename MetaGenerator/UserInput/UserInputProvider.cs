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
    public class UserInputProvider : IUserInputProvider
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

        public void AddValidation(string variableName, IValidation validation)
        {
            _knownUserInputs[variableName].Validations.Add(validation);
        }

        public string Value(string variableName)
        {
            return _knownUserInputs
                .TryGetValue(key: variableName)
                .Match(
                none: () => throw new Exception($"Variable '{variableName}' is not a registered user input."),
                some: userInput => userInput.Value).OrElse("BAD");
        }

        private List<Variable> GetVariablesFromTemplate(string templateName)
        {
            using (var stream = _templateLoader.GetTemplateStream(templateName))
            {
                return (List<Variable>)_jsonSerializer
                    .ReadObject(stream);
            }
        }

        private void RegisterVariablesFromJson(Variable variable)
        {
            var builder = _newInputDescriptionBuilder();

            RegisterVariableName(variable.Name, builder);
            RegsiterVariableQuestion(variable.Question, builder);
            RegsiterVariableDescription(variable.Description, builder);
            RegisterVariableType(variable.GetVariableType(), builder);
            RegisterSelectionValues(variable, builder);

            RegisterVariable(builder.Build());
        }

        private void RegisterSelectionValues(Variable variable, UserInputDescriptionBuilder builder)
        {
            if (variable.GetVariableType() == VariableType.Selection && (variable.SelectionValues == null || variable.SelectionValues.Count == 0))
            {
                throw new Exception("If the variable type is selection, there must be at least one selection value.");
            }

            if (variable.GetVariableType() != VariableType.Selection && variable.SelectionValues != null && variable.SelectionValues.Count > 0)
            {
                throw new Exception("You have specified values for a selection, but the type is not a selection.");
            }

            if (variable.SelectionValues != null && variable.SelectionValues.Any(selectionValue => string.IsNullOrEmpty(selectionValue.Value)))
            {
                throw new Exception("All selections value must have a valid string value!");
            }

            if (variable.GetVariableType() == VariableType.Selection && variable.SelectionValues != null && variable.SelectionValues.Count > 0)
            {
                builder.SetSelectionValues(variable.SelectionValues);
            }
        }

        private void RegsiterVariableQuestion(string? variableQuestion, UserInputDescriptionBuilder builder)
        {
            if (variableQuestion != null)
            {
                builder.SetVariableQuestion(variableQuestion);
            }
        }

        private void RegsiterVariableDescription(string? variableDescription, UserInputDescriptionBuilder builder)
        {
            if (variableDescription != null)
            {
                builder.SetVariableDescription(variableDescription);
            }
        }

        private void RegisterVariableType(VariableType variableType, UserInputDescriptionBuilder builder)
        {
            builder.SetVariableType(variableType);
        }

        private static void RegisterVariableName(string? variableName, UserInputDescriptionBuilder builder)
        {
            if (variableName == null)
            {
                throw new Exception("A variable must have a name.");
            }

            builder.SetVariableName(variableName);
        }
    }
}