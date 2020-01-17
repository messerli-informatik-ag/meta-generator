using System.Collections.Generic;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface IVariableProvider
    {
        void RegisterValue(string variable, string value);

        Dictionary<string, string> GetVariableValues();
    }
}
