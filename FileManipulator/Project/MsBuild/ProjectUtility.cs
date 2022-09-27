using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project.MsBuild;

internal static class ProjectUtility
{
    public static MsBuildProject OpenProjectForEditing(string projectFilePath, ProjectCollection projectCollection)
    {
        var projectRootElement = ProjectRootElement.Open(projectFilePath, projectCollection, preserveFormatting: true);
        return new MsBuildProject(projectRootElement);
    }
}
