using System;
using System.Collections.Generic;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class UserInputDescriptionBuilder
    {
        private readonly List<IValidation> _validations = new();
        private Option<string> _variableName;
        private Option<string> _variableQuestion;
        private Option<string> _variableDescription;
        private VariableType _variableType;
        private Func<bool> _isNeededPredicate;
        private Option<List<SelectionValue>> _variableSelectionValues;

        public UserInputDescriptionBuilder()
        {
            _isNeededPredicate = UserInputDescription.AlwaysNeeded;
        }

        public UserInputDescription Build()
        {
            return _variableName
                .Match(
                none: () => throw new Exception("Variable Name needs to be set with SetVariableName before building!"),
                some: name => new UserInputDescription(name, _variableQuestion, _variableDescription, _variableType, _isNeededPredicate, _variableSelectionValues, _validations));
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
