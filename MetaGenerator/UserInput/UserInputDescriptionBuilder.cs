using System;
using System.Collections.Generic;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class UserInputDescriptionBuilder
    {
        private string? _variableName;
        private string? _variableQuestion;
        private string? _variableDescription;
        private VariableType _variableType;
        private Func<bool> _isNeededPredicate;
        private List<SelectionValue>? _variableSelectionValues;
        private List<IValidation> _validations = new List<IValidation>();

        public UserInputDescriptionBuilder()
        {
            _isNeededPredicate = UserInputDescription.AlwaysNeeded;
        }

        public UserInputDescription Build()
        {
            return _variableName == null
                ? throw new Exception("Variable Name needs to be set with SetVariableName before building!")
                : new UserInputDescription(_variableName, _variableQuestion, _variableDescription, _variableType, _isNeededPredicate, _variableSelectionValues, _validations);
        }

        public UserInputDescriptionBuilder SetIsNeededPredicate(Func<bool> isNeededPredicate)
        {
            _isNeededPredicate = isNeededPredicate;

            return this;
        }

        public UserInputDescriptionBuilder SetVariableName(string variableName)
        {
            _variableName = variableName;

            return this;
        }

        public UserInputDescriptionBuilder SetVariableDescription(string variableDescription)
        {
            _variableDescription = variableDescription;

            return this;
        }

        public UserInputDescriptionBuilder SetVariableQuestion(string variableQuestion)
        {
            _variableQuestion = variableQuestion;

            return this;
        }

        public void SetValidation(IValidation validation)
        {
            _validations.Add(validation);
        }

        public UserInputDescriptionBuilder SetVariableType(VariableType variableType)
        {
            _variableType = variableType;

            return this;
        }

        public UserInputDescriptionBuilder SetSelectionValues(List<SelectionValue> variableSelectionValues)
        {
            _variableSelectionValues = variableSelectionValues;

            return this;
        }
    }
}