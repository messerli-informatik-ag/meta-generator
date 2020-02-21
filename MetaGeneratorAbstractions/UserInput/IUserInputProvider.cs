using System.Collections.Generic;

namespace Messerli.MetaGeneratorAbstractions.UserInput
{
    public interface IUserInputProvider
    {
        IUserInputDescription this[string variableName] { get; }

        string Value(string variableName);

        Dictionary<string, string> GetVariableValues();

        void RegisterVariablesFromTemplate(string templateName);

        void AskUser(Dictionary<string, string> userArguments);

        IEnumerable<IUserInputDescription> GetUserInputDescriptions();

        void Clear();
    }
}