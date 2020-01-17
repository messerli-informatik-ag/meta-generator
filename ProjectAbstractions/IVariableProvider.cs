using System;
using System.Collections.Generic;
using System.Text;

namespace Messerli.ProjectAbstractions
{
    public interface IVariableProvider
    {
        void RegisterValue(string variable, string value);

        Dictionary<string, string> GetVariableValues();
    }
}
