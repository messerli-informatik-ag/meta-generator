using System.Collections.Generic;

namespace Messerli.ProjectAbstractions.UserInput
{
    public interface IUserInputProvider
    {
        string Value(string variableName);

        void RegisterVariablesFromTemplate(string templateName);

        void AskUser();

        void AddValidation(string variableName, IValidation validation);

        Dictionary<string, string> GetVariableValues();
    }
}