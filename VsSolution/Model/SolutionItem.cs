namespace Messerli.VsSolution.Model;

public class SolutionItem
{
    public SolutionItem(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public string Value { get; }
}
