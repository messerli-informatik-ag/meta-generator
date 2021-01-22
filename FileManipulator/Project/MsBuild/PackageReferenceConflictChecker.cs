using System;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.FileManipulatorAbstractions.Project;
using Microsoft.Build.Evaluation;
using static Messerli.FileManipulator.Project.MsBuild.Constant;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project.MsBuild
{
    internal sealed class PackageReferenceConflictChecker : IPackageReferenceConflictChecker
    {
        public PackageReferenceConflictResult CheckForConflict(MsBuildProject project, PackageReference packageReference)
            => GetExistingItem(project, packageReference)
                .Match(
                    none: new PackageReferenceConflictResult.NoExisting(),
                    some: item => ValidateExistingPackageReference(packageReference, item));

        private static Option<ProjectItem> GetExistingItem(MsBuildProject project, PackageReference packageReference)
            => project
                .Items
                .Where(item => item.ItemType == PackageReferenceTypeTag)
                .SingleOrNone(item => item.EvaluatedInclude == packageReference.Name);

        private static PackageReferenceConflictResult ValidateExistingPackageReference(PackageReference packageReference, ProjectItem item)
        {
            var currentVersion = GetVersionMetadata(item);
            return currentVersion == packageReference.Version
                ? (PackageReferenceConflictResult)new PackageReferenceConflictResult.ExistingIsCompatible()
                : new PackageReferenceConflictResult.ExistingIsIncompatible(currentVersion);
        }

        private static string GetVersionMetadata(ProjectItem item)
            => item.GetMetadataValue(VersionMetadataAttribute)
                  ?? throw new InvalidOperationException($"Existing package reference '{item.EvaluatedInclude}' is missing a version");
    }
}
