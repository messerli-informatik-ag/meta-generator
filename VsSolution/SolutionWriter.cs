using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Parser.GlobalSection;

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
            result.AppendLine($"Project(\"{project.TypeGuid.SolutionFormat()}\") = \"{project.ProjectName}\", \"{project.ProjectPath}\", \"{project.ProjectGuid.SolutionFormat()}\"");

            if (project.SolutionItems.Any())
            {
                WriteSolutionItems(project.SolutionItems, result);
            }

            result.AppendLine("EndProject");
        }

        private static void WriteSolutionItems(List<SolutionItem> solutionItems, StringBuilder result)
        {
            result.AppendLine("\tProjectSection(SolutionItems) = preProject");

            foreach (var solutionItem in solutionItems)
            {
                result.AppendLine($"\t\t{solutionItem.Name} = {solutionItem.Value}");
            }

            result.AppendLine("\tEndProjectSection");
        }
    }
}