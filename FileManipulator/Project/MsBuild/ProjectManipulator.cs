using System;
using System.Collections.Generic;
using System.Linq;
using Funcky;
using Funcky.Monads;
using Messerli.FileManipulatorAbstractions.Project;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using static Messerli.FileManipulator.Project.MsBuild.Constant;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project.MsBuild
{
    internal sealed class ProjectManipulator : IProjectManipulator
    {
        private readonly IProjectSdkManipulator _projectSdkManipulator;

        private readonly ICentralPackageVersionsManipulator _centralPackageVersionsManipulator;

        private readonly IPackageReferenceConflictChecker _packageReferenceConflictChecker;

        public ProjectManipulator(
            IProjectSdkManipulator projectSdkManipulator,
            ICentralPackageVersionsManipulator centralPackageVersionsManipulator,
            IPackageReferenceConflictChecker packageReferenceConflictChecker)
        {
            _projectSdkManipulator = projectSdkManipulator;
            _centralPackageVersionsManipulator = centralPackageVersionsManipulator;
            _packageReferenceConflictChecker = packageReferenceConflictChecker;
        }

        private delegate void AddVersionMetadata(ProjectItemElement item, PackageReference packageReference);

        public void ManipulateProject(string projectFilePath, ProjectModification modification)
        {
            using var projectCollection = new ProjectCollection();
            var project = ProjectUtility.OpenProjectForEditing(projectFilePath, projectCollection);
            _projectSdkManipulator.AddSdksToProject(project, modification.SdksToAdd);
            AddPackageReferencesToProject(project, modification.PackageReferencesToAdd);
            project.Save();
        }

        private void AddPackageReferencesToProject(MsBuildProject project, IEnumerable<PackageReference> packageReferences)
        {
            if (packageReferences.Any())
            {
                AddPackageReferencesToProjectWithoutGuard(project, packageReferences);
            }
        }

        private void AddPackageReferencesToProjectWithoutGuard(MsBuildProject project, IEnumerable<PackageReference> packageReferences)
        {
            var usesCentralPackages = _centralPackageVersionsManipulator.ProjectUsesCentralPackageVersionsSdk(project);
            var addVersionMetadataToItem = usesCentralPackages
                ? (AddVersionMetadata)NoOperation
                : AddVersionMetadataToItem;

            var itemGroup = project.GetItemGroupWithItemOfTypeOrCreateNew(PackageReferenceTypeTag);

            void AddPackageReferenceCurried(PackageReference packageReference) => AddPackageReference(itemGroup, packageReference, addVersionMetadataToItem);

            foreach (var packageReference in packageReferences)
            {
                AddPackageReferenceWithValidation(project, packageReference, AddPackageReferenceCurried);
            }

            if (usesCentralPackages)
            {
                _centralPackageVersionsManipulator.AddPackageReferencesToGlobalPackages(project, packageReferences);
            }
        }

        private void AddPackageReferenceWithValidation(MsBuildProject project, PackageReference packageReference, Action<PackageReference> addPackageReference)
        {
            Action action = _packageReferenceConflictChecker.CheckForConflict(project, packageReference) switch
            {
                PackageReferenceConflictResult.NoExisting _ => () => addPackageReference(packageReference),
                PackageReferenceConflictResult.ExistingIsIncompatible result => () => throw new ConflictingPackageReferenceException(packageReference, result.Version),
                PackageReferenceConflictResult.ExistingIsCompatible _ => Functional.NoOperation,
                var result => throw new InvalidOperationException($"Enum variant {result.GetType().Name} is not supported"),
            };
            action();
        }

        private static void AddPackageReference(ProjectItemGroupElement itemGroup, PackageReference packageReference, AddVersionMetadata addVersionMetadata)
        {
            var item = itemGroup.AddItem(PackageReferenceTypeTag, packageReference.Name);
            addVersionMetadata(item, packageReference);
            AddAssetsListMetadataToPackageReference(
                item,
                PrivateAssetsMetadataAttribute,
                packageReference.PrivateAssets);
            AddAssetsListMetadataToPackageReference(
                item,
                IncludeAssetsMetadataAttribute,
                packageReference.IncludeAssets);
            AddAssetsListMetadataToPackageReference(
                item,
                ExcludeAssetsMetadataAttribute,
                packageReference.ExcludeAssets);
        }

        private static void AddVersionMetadataToItem(ProjectItemElement item, PackageReference packageReference)
            => item.AddMetadataAsAttribute(VersionMetadataAttribute, packageReference.Version);

        private static void AddAssetsListMetadataToPackageReference(
            ProjectItemElement item,
            string attributeName,
            Option<DependencyAssets> assetList)
            => assetList.AndThen(list => item.AddMetadataAsAttribute(attributeName, MapDependencyAssetsToString(list)));

        private static string MapDependencyAssetsToString(DependencyAssets dependencyAssets)
            => dependencyAssets switch
            {
                DependencyAssets.All _ => "all",
                DependencyAssets.None _ => "none",
                DependencyAssets.List list => string.Join(ListSeparator, list.Select(MapAssetNameToString)),
                _ => throw new InvalidOperationException($"Enum variant {dependencyAssets.GetType().Name} is not supported"),
            };

        private static string MapAssetNameToString(DependencyAssetName dependencyAssetName)
            => dependencyAssetName switch
            {
                DependencyAssetName.Compile => "compile",
                DependencyAssetName.Runtime => "runtime",
                DependencyAssetName.ContentFiles => "contentFiles",
                DependencyAssetName.Build => "build",
                DependencyAssetName.BuildMultiTargeting => "buildMultitargeting",
                DependencyAssetName.BuildTransitive => "buildTransitive",
                DependencyAssetName.Analyzers => "analyzers",
                DependencyAssetName.Native => "native",
                _ => throw new InvalidOperationException($"Enum variant {dependencyAssetName} is not supported"),
            };

        private static void NoOperation<T1, T2>(T1 paramOne, T2 paramTwo)
        {
        }
    }
}
