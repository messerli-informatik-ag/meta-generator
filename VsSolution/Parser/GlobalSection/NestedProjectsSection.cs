using System;
using System.Linq;
using System.Text;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.GlobalSection
{
    internal class NestedProjectsSection : IGlobalSection
    {
        public void Parse(TokenWalker tokenWalker, Solution solution)
        {
            while (tokenWalker.NextIs<EndGlobalSectionToken>() == false)
            {
                tokenWalker.ConsumeAllWhiteSpace();
                var child = tokenWalker.ConsumeGuid();
                tokenWalker.ConsumeAllWhiteSpace();
                tokenWalker.Consume<AssignToken>();
                tokenWalker.ConsumeAllWhiteSpace();
                var parent = tokenWalker.ConsumeGuid();
                tokenWalker.ConsumeAllWhiteSpace();

                solution.ProjectNesting.Add(new NestedProject(parent, child));
            }
        }

        public void Serialize(Solution solution, StringBuilder result)
        {
            foreach (var nestedProject in solution.ProjectNesting)
            {
                result.AppendLine($"\t\t{nestedProject.Child.SolutionFormat()} = {nestedProject.Parent.SolutionFormat()}");
            }
        }

        public bool Exists(Solution solution)
        {
            return solution.ProjectNesting.Any();
        }
    }
}