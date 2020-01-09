using System.Linq;
using System.Text;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.GlobalSection
{
    internal class SolutionPropertiesSection : IGlobalSection
    {
        public void Parse(TokenWalker tokenWalker, Solution solution)
        {
            while (tokenWalker.NextIs<EndGlobalSectionToken>() == false)
            {
                var property = tokenWalker.ConsumeBareString();
                tokenWalker.Consume<AssignToken>();
                var propertyValue = tokenWalker.ConsumeBareString();
                tokenWalker.ConsumeAllWhiteSpace();

                solution.Properties.Add(new SolutionProperty(property, propertyValue));
            }
        }

        public void Serialize(Solution solution, StringBuilder result)
        {
            foreach (var property in solution.Properties)
            {
                result.AppendLine($"\t\t{property.Property} = {property.PropertyValue}");
            }
        }

        public bool Exists(Solution solution)
        {
            return solution.Properties.Any();
        }
    }
}
