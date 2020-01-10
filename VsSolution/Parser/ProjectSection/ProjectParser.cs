using System;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.ProjectSection
{
    public class ProjectParser
    {
        internal void Parse(TokenWalker tokenWalker, Solution solution)
        {
            tokenWalker.Consume<BeginProjectToken>();

            var project = ParseProject(tokenWalker);
            ParseProjectSection(tokenWalker, project);
            solution.Projects.Add(project);

            tokenWalker.Consume<EndProjectToken>();
            tokenWalker.Consume<NewLineToken>();
        }

        private void CheckLoadingOrder(string loadingOrder)
        {
            if (LoadingOrders.IsValidProjectLoadingOrder(loadingOrder) == false)
            {
                throw new ParseException($"Unknown loading Order '{loadingOrder}'");
            }
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

            return new Project(projectName, projectPath, typeGuid, projectGuid);
        }

        private void ParseProjectSection(TokenWalker tokenWalker, Project project)
        {
            tokenWalker.ConsumeAllWhiteSpace();
            while (tokenWalker.NextIs<BeginProjectSectionToken>())
            {
                tokenWalker.Consume<BeginProjectSectionToken>();

                tokenWalker.Consume<OpenParenthesisToken>();
                var sectionType = Enum.Parse<ProjectSectionType>(tokenWalker.ConsumeWord());
                tokenWalker.Consume<ClosedParenthesisToken>();

                tokenWalker.ConsumeAllWhiteSpace();
                tokenWalker.Consume<AssignToken>();

                tokenWalker.ConsumeAllWhiteSpace();
                CheckLoadingOrder(tokenWalker.ConsumeWord());
                tokenWalker.Consume<NewLineToken>();

                ParseProjectSection(sectionType, tokenWalker, project);

                tokenWalker.Consume<EndProjectSectionToken>();
                tokenWalker.ConsumeAllWhiteSpace();
            }
        }

        private void ParseProjectSection(ProjectSectionType sectionType, TokenWalker tokenWalker, Project project)
        {
            var projectSection = ProjectSectionTypeFactory.Create(sectionType);

            projectSection.Parse(tokenWalker, project);
        }
    }
}
