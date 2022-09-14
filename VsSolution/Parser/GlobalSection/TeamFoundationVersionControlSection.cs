using System.Linq;
using System.Text;
using Messerli.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.GlobalSection;

internal class TeamFoundationVersionControlSection : IGlobalSection
{
    public void Parse(TokenWalker tokenWalker, Solution solution)
    {
        while (tokenWalker.NextIs<EndGlobalSectionToken>() == false)
        {
            var property = tokenWalker.ConsumeVariable();

            solution.TfsControlProperties.Add(new TfsControlProperty(property.Key, property.Value));
        }
    }

    public void Serialize(Solution solution, StringBuilder result)
    {
        foreach (var property in solution.TfsControlProperties)
        {
            result.AppendLine($"\t\t{property.PropertyKey} = {property.PropertyValue}");
        }
    }

    public bool Exists(Solution solution)
    {
        return solution.TfsControlProperties.Any();
    }
}
