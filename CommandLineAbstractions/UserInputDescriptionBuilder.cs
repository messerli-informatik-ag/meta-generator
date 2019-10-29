using System;

namespace Messerli.CommandLineAbstractions
{
    public class UserInputDescriptionBuilder
    {
        private Func<bool> _isNeededPredicate;
        private string? _variableName;

        public UserInputDescriptionBuilder()
        {
            _isNeededPredicate = UserInputDescription.AlwaysNeeded;
        }

        public void SetIsNeededPredicate(Func<bool> isNeededPredicate)
        {
            _isNeededPredicate = isNeededPredicate;
        }

        public void SetVariableName(string variableName)
        {
            _variableName = variableName;
        }

        public UserInputDescription Build()
        {
            return _variableName == null
                ? throw new Exception("Variable Name needs to be set with SetVariableName before building!")
                : new UserInputDescription(_variableName, _isNeededPredicate);
        }
    }
}