using System;

namespace Messerli.MetaGenerator.UserInput;

internal class ValidationName
{
    public ValidationName(string validation)
    {
        var parts = validation.Split('.');

        if (parts.Length == 2)
        {
            Class = parts[0];
            Property = parts[1];
        }
        else
        {
            throw new Exception($"The validation '{validation}' should be declared in the form StaticClass.StaticProperty without a namespace.");
        }
    }

    public string Class { get; }

    public string Property { get; }

    public override string ToString()
        => $"{Class}.{Property}";
}
