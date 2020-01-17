using System;
using System.Collections.Generic;
using Messerli.ProjectAbstractions;
using Messerli.ProjectAbstractions.Json;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
{
    public class UserInputDescriptionBuilder
    {
        private string? _variableName;
        private string? _variableQuestion;
        private string? _variableDescription;
        private VariableType _variableType;
        private Func<bool> _isNeededPredicate;
        private List<SelectionValue>? _variableSelectionValues;

        public UserInputDescriptionBuilder()
        {
            _isNeededPredicate = UserInputDescription.AlwaysNeeded;
        }

        public UserInputDescription Build()
        {
            return _variableName == null
                ? throw new Exception("Variable Name needs to be set with SetVariableName before building!")
                : new UserInputDescription(_variableName, _variableQuestion, _variableDescription, _variableType, _isNeededPredicate, _variableSelectionValues);
        }

        public void SetIsNeededPredicate(Func<bool> isNeededPredicate)
        {
            _isNeededPredicate = isNeededPredicate;
        }

        public void SetVariableName(string variableName)
        {
            _variableName = variableName;
        }

        public void SetVariableDescription(string variableDescription)
        {
            _variableDescription = variableDescription;
        }

        public void SetVariableQuestion(string variableQuestion)
        {
            _variableQuestion = variableQuestion;
        }

        public void SetVariableType(VariableType variableType)
        {
            _variableType = variableType;
        }

        public void SetSelectionValues(List<SelectionValue> variableSelectionValues)
        {
            _variableSelectionValues = variableSelectionValues;
        }
    }
}