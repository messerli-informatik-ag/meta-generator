using System;
using System.Collections.Generic;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    public class UserInputDescription : IUserInputDescription
    {
        internal UserInputDescription(string variableName, string? variableQuestion, string? variableDescription, VariableType variableType, Func<bool> isNeededPredicate, List<SelectionValue>? variableSelectionValues)
        {
            VariableName = variableName;
            VariableQuestion = variableQuestion;
            VariableDescription = variableDescription;
            VariableType = variableType;
            VariableName = variableName;
            IsNeeded = new Lazy<bool>(isNeededPredicate);
            VariableSelectionValues = variableSelectionValues ?? new List<SelectionValue>();
            Validations = new List<IValidation>();
        }

        public List<SelectionValue> VariableSelectionValues { get; set; }

        public string VariableName { get; }

        public string? VariableQuestion { get; }

        public string? VariableDescription { get; }

        public VariableType VariableType { get; }

        public Option<string> Value { get; set; }

        public Lazy<bool> IsNeeded { get; }

        public List<IValidation> Validations { get; }

        public static Func<bool> AlwaysNeeded { get; } = () => true;
    }
}