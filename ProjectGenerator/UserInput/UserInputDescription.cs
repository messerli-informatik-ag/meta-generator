using System;
using System.Collections.Generic;
using Funcky.Monads;
using Messerli.ProjectAbstractions.Json;
using Messerli.ProjectAbstractions.UserInput;

namespace Messerli.ProjectGenerator.UserInput
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
        }

        public List<SelectionValue> VariableSelectionValues { get; set; }

        public string VariableName { get; }

        public string? VariableQuestion { get; }

        public string? VariableDescription { get; }

        public VariableType VariableType { get; }

        public Option<string> Value { get; set; }

        public Lazy<bool> IsNeeded { get; }

        public static Func<bool> AlwaysNeeded { get; } = () => true;
    }
}