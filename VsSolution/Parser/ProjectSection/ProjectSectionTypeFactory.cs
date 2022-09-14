using System;

namespace Messerli.VsSolution.Parser.ProjectSection;

internal static class ProjectSectionTypeFactory
{
    public static IProjectSection Create(ProjectSectionType sectionType)
    {
        return sectionType switch
        {
            ProjectSectionType.SolutionItems => new SolutionItemsSection(),
            ProjectSectionType.ProjectDependencies => new ProjectDependenciesSection(),
            _ => throw new Exception($"Unknown project section type : {sectionType}"),
        };
    }
}
