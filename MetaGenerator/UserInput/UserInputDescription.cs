using System;
using System.Collections.Generic;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGenerator.UserInput
{
    internal class UserInputDescription : IUserInputDescription
    {
        internal UserInputDescription(string variableName, Option<string> variableQuestion, Option<string> variableDescription, VariableType variableType, Func<bool> isNeededPredicate, Option<List<SelectionValue>> variableSelectionValues, List<IValidation> validations)
        {
            VariableName = variableName;
            VariableQuestion = variableQuestion;
            VariableDescription = variableDescription;
            VariableType = variableType;
            VariableName = variableName;
            IsNeeded = new Lazy<bool>(isNeededPredicate);
            VariableSelectionValues = variableSelectionValues.GetOrElse(new List<SelectionValue>());
            Validations = validations;
        }

        public List<SelectionValue> VariableSelectionValues { get; set; }

        public string VariableName { get; }

        public Option<string> VariableQuestion { get; }

        public Option<string> VariableDescription { get; }

        public VariableType VariableType { get; }

        public Option<string> Value { get; set; }

        public Lazy<bool> IsNeeded { get; }

        public List<IValidation> Validations { get; }

        public static Func<bool> AlwaysNeeded { get; } = () => true;
    }
}
