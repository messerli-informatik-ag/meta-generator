using System;

namespace Messerli.VsSolution.Model
{
    internal class VersionVariable
    {
        public VersionVariable(string name, Version value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public Version Value { get; }
    }
}
