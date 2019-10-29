using System;
using System.Collections.Generic;

namespace Messerli.CommandLineAbstractions
{
    public interface IUserInputProvider
    {
        string Value(string variableName);

        void RegisterVariable(UserInputDescription description);

        void AskUser();

        Dictionary<string, string> View();
    }
}