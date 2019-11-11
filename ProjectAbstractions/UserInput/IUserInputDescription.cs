using System;
using System.Collections.Generic;
using Funcky.Monads;
using Messerli.ProjectAbstractions.Json;

namespace Messerli.ProjectAbstractions.UserInput
{
    public interface IUserInputDescription
    {
        List<SelectionValue> VariableSelectionValues { get; set; }

        string VariableName { get; }

        string? VariableQuestion { get; }

        string? VariableDescription { get; }

        VariableType VariableType { get; }

        Option<string> Value { get; set; }

        Lazy<bool> IsNeeded { get; }
    }
}