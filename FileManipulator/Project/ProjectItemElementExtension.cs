using Microsoft.Build.Construction;

namespace Messerli.FileManipulator.Project;

public static class ProjectItemElementExtension
{
    public static ProjectMetadataElement AddMetadataAsAttribute(this ProjectItemElement itemElement, string name, string unevaluatedValue)
        => itemElement.AddMetadata(name, unevaluatedValue, expressAsAttribute: true);
}
