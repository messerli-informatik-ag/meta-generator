using System;
using System.Linq;
using System.Text;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.ProjectSection
{
    internal class ProjectDependenciesSection : IProjectSection
    {
        public void Parse(TokenWalker tokenWalker, Project project)
        {
            while (tokenWalker.NextIs<EndProjectSectionToken>() == false)
            {
                var variable = tokenWalker.ConsumeVariable();
                tokenWalker.ConsumeAllWhiteSpace();

                project.Dependencies.Add(new Dependency(Guid.Parse(variable.Key), Guid.Parse(variable.Value)));
            }
        }

        public void Serialize(Project project, StringBuilder result)
        {
            foreach (var dependency in project.Dependencies)
            {
                result.AppendLine($"\t\t{dependency.Dependent.SolutionFormat()} = {dependency.Dependee.SolutionFormat()}");
            }
        }

        public bool Exists(Project project)
        {
            return project.Dependencies.Any();
        }
    }
}