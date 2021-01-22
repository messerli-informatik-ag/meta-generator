using System;
using System.Collections.Generic;
using System.Linq;
using Funcky;
using Funcky.Extensions;
using Funcky.Monads;
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
            var packagesProject = ProjectUtility.OpenProjectForEditing(GetCentralPackagesFile(project), projectCollection);
            AddPackageReferences(packagesProject, packageReferences);
            packagesProject.Save();
        }

        private static void AddPackageReferences(MsBuildProject packagesProject, IEnumerable<PackageReference> packageReferences)
        {
            var itemGroup = packagesProject.GetItemGroupWithItemOfTypeOrCreateNew(PackageReferenceTypeTag);
            foreach (var packageReference in packageReferences)
            {
                Action action = CheckForConflict(packagesProject, packageReference) switch
                {
                    PackageReferenceConflictResult.NoExisting _ => () => AddPackageReference(itemGroup, packageReference),
                    PackageReferenceConflictResult.ExistingIsCompatible _ => Functional.NoOperation,
                    PackageReferenceConflictResult.ExistingIsIncompatible result => throw new ConflictingPackageReferenceException(packageReference, result.Version),
                    var result => throw new InvalidOperationException($"Enum variant {result.GetType().Name} is not supported"),
                };
                action();
            }
        }

        private static PackageReferenceConflictResult CheckForConflict(MsBuildProject project, PackageReference packageReference)
            => GetExistingPackageReference(project, packageReference)
            .Match(
                none: () => new PackageReferenceConflictResult.NoExisting(),
                some: existing => ValidateExistingPackageReference(existing, packageReference));

        private static PackageReferenceConflictResult ValidateExistingPackageReference(ProjectItemElement item, PackageReference packageReference)
        {
            var existingVersion = GetVersionFromMetadata(item);
            return existingVersion == packageReference.Version
                ? (PackageReferenceConflictResult)new PackageReferenceConflictResult.ExistingIsCompatible()
                : new PackageReferenceConflictResult.ExistingIsIncompatible(existingVersion);
        }

        private static void AddPackageReference(ProjectElementContainer itemGroup, PackageReference packageReference)
        {
            var centralPackageItem = itemGroup.ContainingProject.CreateItemElement(PackageReferenceTypeTag);
            centralPackageItem.Update = packageReference.Name;
            itemGroup.AppendChild(centralPackageItem);
            centralPackageItem.AddMetadataAsAttribute(VersionMetadataAttribute, packageReference.Version);
        }

        private static Option<ProjectItemElement> GetExistingPackageReference(MsBuildProject project, PackageReference packageReference)
            => project
                .Xml
                .Items
                .Where(item => item.ItemType == PackageReferenceTypeTag)
                .FirstOrNone(item => item.Update == packageReference.Name);

        private static string GetVersionFromMetadata(ProjectItemElement item)
            => item.Metadata
                .Where(m => m.Name == VersionMetadataAttribute)
                .Select(m => m.Value)
                .FirstOrNone()
                .GetOrElse(() => throw new InvalidOperationException($"Package reference '{item.Update}' is missing a version"));

        private static bool HasCentralPackageVersionsEnabled(MsBuildProject project)
            => project.GetPropertyValue(EnableCentralPackageVersionsProperty) != FalseAsString;

        private static string GetCentralPackagesFile(MsBuildProject project)
            => project.GetPropertyValue(CentralPackagesFileProperty);

        private static bool IsCentralPackageVersionsSdkImport(ResolvedImport import)
            => import.SdkResult is { } sdk &&
               sdk.SdkReference.Name == CentralPackageVersionsSdk;
    }
}
