using System.Linq;
using System.Text;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.ProjectSection
{
    public class SolutionItemsSection : IProjectSection
    {
        public void Parse(TokenWalker tokenWalker, Project project)
        {
            while (tokenWalker.NextIs<EndProjectSectionToken>() == false)
            {
                var variable = tokenWalker.ConsumeVariable();
                tokenWalker.ConsumeAllWhiteSpace();

                project.SolutionItems.Add(new SolutionItem(variable.Key, variable.Value));
            }
        }

        public void Serialize(Project project, StringBuilder result)
        {
            foreach (var solutionItem in project.SolutionItems)
            {
                result.AppendLine($"\t\t{solutionItem.Name} = {solutionItem.Value}");
            }
        }

        public bool Exists(Project project)
        {
            return project.SolutionItems.Any();
        }
    }
}