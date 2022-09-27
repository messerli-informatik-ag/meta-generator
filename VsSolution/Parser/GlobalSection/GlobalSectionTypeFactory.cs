using System;
using Messerli.VsSolution.Model;

namespace Messerli.VsSolution.Parser.GlobalSection;

internal static class GlobalSectionTypeFactory
{
    public static IGlobalSection Create(GlobalSectionType sectionType)
    {
        return sectionType switch
        {
            GlobalSectionType.SolutionConfigurationPlatforms => new SolutionConfigurationPlatformsSection(),
            GlobalSectionType.ProjectConfigurationPlatforms => new ProjectConfigurationPlatformsSection(),
            GlobalSectionType.SolutionProperties => new SolutionPropertiesSection(),
            GlobalSectionType.NestedProjects => new NestedProjectsSection(),
            GlobalSectionType.ExtensibilityGlobals => new ExtensibilityGlobalsSection(),
            GlobalSectionType.TeamFoundationVersionControl => new TeamFoundationVersionControlSection(),
            _ => throw new Exception($"Unknown global section type : {sectionType}"),
        };
    }
}
