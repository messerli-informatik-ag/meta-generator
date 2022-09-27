using System.Collections.Generic;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project.MsBuild;

internal interface IProjectSdkManipulator
{
    void AddSdksToProject(MsBuildProject project, IEnumerable<string> sdks);
}
