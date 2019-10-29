using System;
using Funcky.Monads;

namespace Messerli.CommandLineAbstractions
{
    public class UserInputDescription
    {
        public UserInputDescription(string variableName, Func<bool> isNeededPredicate)
        {
            IsNeeded = new Lazy<bool>(isNeededPredicate);
            VariableName = variableName;
        }

        public string VariableName { get; }

        public Option<string> Value { get; set; }

        public Lazy<bool> IsNeeded { get; }

        public static Func<bool> AlwaysNeeded { get; } = () => true;
    }
}