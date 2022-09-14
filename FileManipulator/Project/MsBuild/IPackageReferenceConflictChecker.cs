using Messerli.FileManipulatorAbstractions.Project;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project.MsBuild;

internal interface IPackageReferenceConflictChecker
{
    PackageReferenceConflictResult CheckForConflict(MsBuildProject project, PackageReference packageReference);
}
