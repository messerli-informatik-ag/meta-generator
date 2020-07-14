using System.Collections.Generic;
using Messerli.FileManipulatorAbstractions.Project;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project.MsBuild
{
    internal interface ICentralPackageVersionsManipulator
    {
        bool ProjectUsesCentralPackageVersionsSdk(MsBuildProject project);

        void AddPackageReferencesToGlobalPackages(MsBuildProject project, IEnumerable<PackageReference> packageReferences);
    }
}