using System.Text;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Parser.GlobalSection;
using Messerli.VsSolution.Parser.ProjectSection;

namespace Messerli.VsSolution
{
    internal class SolutionWriter
    {
        public static SolutionWriter Create()
        {
            return new SolutionWriter();
        }

        internal string Serialize(Solution solution)
        {
            var result = new StringBuilder();

            result.AppendLine($"Microsoft Visual Studio Solution File, Format Version {solution.FormatVersion}.00");
            result.AppendLine($"# Visual Studio Version {solution.VisualStudioVersion.Major}");
            result.AppendLine($"VisualStudioVersion = {solution.VisualStudioVersion}");
            result.AppendLine($"MinimumVisualStudioVersion = {solution.MinimumVisualStudioVersion}");

            foreach (var project in solution.Projects)
            {
                Serialize(project, result);
            }

            result.AppendLine("Global");

            WriteGlobalSection(GlobalSectionType.SolutionConfigurationPlatforms, solution, result, "preSolution");
            WriteGlobalSection(GlobalSectionType.ProjectConfigurationPlatforms, solution, result, "postSolution");
            WriteGlobalSection(GlobalSectionType.SolutionProperties, solution, result, "preSolution");
            WriteGlobalSection(GlobalSectionType.NestedProjects, solution, result, "preSolution");
            WriteGlobalSection(GlobalSectionType.ExtensibilityGlobals, solution, result, "postSolution");
            WriteGlobalSection(GlobalSectionType.TeamFoundationVersionControl, solution, result, "preSolution");

            result.AppendLine("EndGlobal");

            return result.ToString();
        }

        private void WriteGlobalSection(GlobalSectionType sectionType, Solution solution, StringBuilder result, string loadingOrder)
        {
            var globalSection = GlobalSectionTypeFactory.Create(sectionType);

            if (globalSection.Exists(solution))
            {
                result.AppendLine($"\tGlobalSection({sectionType}) = {loadingOrder}");

                globalSection.Serialize(solution, result);

                result.AppendLine("\tEndGlobalSection");
            }
        }

        private void Serialize(Project project, StringBuilder result)
        {
            result.AppendLine($"Project(\"{project.ProjectType.Guid.SolutionFormat()}\") = \"{project.ProjectName}\", \"{project.ProjectPath}\", \"{project.ProjectGuid.SolutionFormat()}\"");

            WriteProjectSection(ProjectSectionType.SolutionItems, project, result, "preProject");
            WriteProjectSection(ProjectSectionType.ProjectDependencies, project, result, "postProject");

            result.AppendLine("EndProject");
        }

        private void WriteProjectSection(ProjectSectionType sectionType, Project project, StringBuilder result, string loadingOrder)
        {
            var projectSection = ProjectSectionTypeFactory.Create(sectionType);

            if (projectSection.Exists(project))
            {
                result.AppendLine($"\tProjectSection({sectionType}) = {loadingOrder}");

                projectSection.Serialize(project, result);

                result.AppendLine("\tEndProjectSection");
            }
        }
    }
}