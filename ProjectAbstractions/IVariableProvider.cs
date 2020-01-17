using System.Collections.Generic;

namespace Messerli.ProjectAbstractions
{
    public interface IVariableProvider
    {
        void RegisterValue(string variable, string value);

        Dictionary<string, string> GetVariableValues();
    }
}
