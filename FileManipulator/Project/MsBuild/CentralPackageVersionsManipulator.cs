using System.Collections.Generic;
using System.Linq;
using Messerli.FileManipulatorAbstractions.Project;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using static Messerli.FileManipulator.Project.MsBuild.Constant;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project.MsBuild
{
    internal sealed class CentralPackageVersionsManipulator : ICentralPackageVersionsManipulator
    {
        private const string CentralPackageVersionsSdk = "Microsoft.Build.CentralPackageVersions";
        private const string EnableCentralPackageVersionsProperty = "EnableCentralPackageVersions";
        private const string FalseAsString = "false";
        private const string CentralPackagesFileProperty = "CentralPackagesFile";

        public bool ProjectUsesCentralPackageVersionsSdk(MsBuildProject project)
            => project.Imports.Any(IsCentralPackageVersionsSdkImport) && HasCentralPackageVersionsEnabled(project);

        public void AddPackageReferencesToGlobalPackages(MsBuildProject project, IEnumerable<PackageReference> packageReferences)
        {
            using var projectCollection = new ProjectCollection();
            var packagesProject = OpenProject(GetCentralPackagesFile(project), projectCollection);
            AddPackageReferences(packagesProject, packageReferences);
            packagesProject.Save();
        }

        private static void AddPackageReferences(MsBuildProject packagesProject, IEnumerable<PackageReference> packageReferences)
        {
            var itemGroup = packagesProject.GetItemGroupWithItemOfTypeOrCreateNew(PackageReferenceTypeTag);
            foreach (var packageReference in packageReferences)
            {
                AddPackageReference(itemGroup, packageReference);
            }
        }

        private static void AddPackageReference(ProjectElementContainer itemGroup, PackageReference packageReference)
        {
            var centralPackageItem = itemGroup.ContainingProject.CreateItemElement(PackageReferenceTypeTag);
            centralPackageItem.Update = packageReference.Name;
            itemGroup.AppendChild(centralPackageItem);
            centralPackageItem.AddMetadataAsAttribute(VersionMetadataAttribute, packageReference.Version);
        }

        private static MsBuildProject OpenProject(string projectFilePath, ProjectCollection projectCollection)
        {
            var projectRootElement = ProjectRootElement.Open(projectFilePath, projectCollection, preserveFormatting: true);
            return new MsBuildProject(projectRootElement);
        }

        private static bool HasCentralPackageVersionsEnabled(MsBuildProject project)
            => project.GetPropertyValue(EnableCentralPackageVersionsProperty) != FalseAsString;

        private static string GetCentralPackagesFile(MsBuildProject project)
            => project.GetPropertyValue(CentralPackagesFileProperty);

        private static bool IsCentralPackageVersionsSdkImport(ResolvedImport import)
            => import.SdkResult is { } sdk &&
               sdk.SdkReference.Name == CentralPackageVersionsSdk;
    }
}