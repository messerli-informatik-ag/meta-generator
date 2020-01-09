using System.Collections.Generic;
using apophis.Lexer;
using Funcky;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser
{
    public class ProjectParser
    {
        private readonly HashSet<string> _possibleLoadingOrders = new HashSet<string> { "preProject", "postProject" };

        internal void Parse(TokenWalker tokenWalker, Solution solution)
        {
            while (tokenWalker.NextIs<BeginProjectToken>())
            {
                tokenWalker.Consume<BeginProjectToken>();

                var project = ParseProject(tokenWalker);
                ParseProjectSection(tokenWalker, project);
                solution.Projects.Add(project);

                tokenWalker.Consume<EndProjectToken>();
                tokenWalker.Consume<NewLineToken>();
            }
        }

        private void CheckLoadingOrder(string loadingOrder)
        {
            if (_possibleLoadingOrders.Contains(loadingOrder) == false)
            {
                throw new ParseException($"Unknown loading Order '{loadingOrder}'");
            }
        }

        private void ParseProjectSection(TokenWalker tokenWalker, Project project)
        {
            tokenWalker.ConsumeAllWhiteSpace();
            if (tokenWalker.NextIs<BeginProjectSectionToken>())
            {
                tokenWalker.Consume<BeginProjectSectionToken>();

                tokenWalker.Consume<OpenParenthesisToken>();
                var sectionType = tokenWalker.ConsumeWord();
                tokenWalker.Consume<ClosedParenthesisToken>();

                tokenWalker.ConsumeAllWhiteSpace();
                tokenWalker.Consume<AssignToken>();

                tokenWalker.ConsumeAllWhiteSpace();
                CheckLoadingOrder(tokenWalker.ConsumeWord());

                ParseConfigurations(sectionType, tokenWalker, project);

                tokenWalker.Consume<EndProjectSectionToken>();
                tokenWalker.Consume<NewLineToken>();
            }
        }

        private Unit ParseConfigurations(string sectionType, TokenWalker tokenWalker, Project project)
        {
            return sectionType switch
            {
                "SolutionItems" => ParseSolutionItems(tokenWalker, project),
                _ => throw new ParseException($"Unknown global section type : {sectionType}")
            };
        }

        private Unit ParseSolutionItems(TokenWalker tokenWalker, Project project)
        {
            while (tokenWalker.NextIs<EndProjectSectionToken>() == false)
            {
                var variable = tokenWalker.ConsumeBareVariable();
                tokenWalker.ConsumeAllWhiteSpace();

                project.SolutionItems.Add(new SolutionItem(variable.Key, variable.Value));
            }

            return default;
        }

        private Project ParseProject(TokenWalker tokenWalker)
        {
            tokenWalker.Consume<OpenParenthesisToken>();
            var typeGuid = tokenWalker.ConsumeGuid();
            tokenWalker.Consume<ClosedParenthesisToken>();

            tokenWalker.ConsumeAllWhiteSpace();
            tokenWalker.Consume<AssignToken>();

            var projectName = tokenWalker.ConsumeString();
            tokenWalker.Consume<CommaToken>();
            var projectPath = tokenWalker.ConsumeString();
            tokenWalker.Consume<CommaToken>();
            var projectGuid = tokenWalker.ConsumeGuid();
            tokenWalker.Consume<NewLineToken>();

            return new Project(projectGuid, typeGuid, projectName, projectPath);
        }
    }
}
