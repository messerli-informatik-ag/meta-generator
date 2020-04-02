using System;
using System.Collections.Generic;
using System.Linq;
using Messerli.FileManipulatorAbstractions.Project;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using static Messerli.FileManipulator.Project.MsBuild.Constant;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project.MsBuild
{
    internal sealed class ProjectManipulator : IProjectManipulator
    {
        private const string CentralPackageVersionsSdk = "Microsoft.Build.CentralPackageVersions";

        private readonly IProjectSdkManipulator _projectSdkManipulator;

        public ProjectManipulator(IProjectSdkManipulator projectSdkManipulator)
        {
            _projectSdkManipulator = projectSdkManipulator;
        }

        public void ManipulateProject(string projectFilePath, ProjectModification modification)
        {
            using var projectCollection = new ProjectCollection();
            var project = OpenProject(projectFilePath, projectCollection);
            var centralPackagesFile = UseCentralPackageVersionsSdk(project) && HasCentralPackageVersionsEnabled(project)
                ? OpenProject(GetCentralPackagesFile(project), projectCollection)
                : null;
            _projectSdkManipulator.AddSdksToProject(project, modification.SdksToAdd);
            AddPackageReferencesToProject(project, centralPackagesFile, modification.PackageReferencesToAdd);
            project.Save();
            centralPackagesFile?.Save();
        }

        private static MsBuildProject OpenProject(string projectFilePath, ProjectCollection projectCollection)
        {
            var projectRootElement = ProjectRootElement.Open(projectFilePath, projectCollection, preserveFormatting: true);
            return new MsBuildProject(projectRootElement);
        }

        private static bool UseCentralPackageVersionsSdk(MsBuildProject project)
            => project.Imports.Any(import => import.SdkResult is { } sdk && sdk.SdkReference.Name == CentralPackageVersionsSdk);

        private static bool HasCentralPackageVersionsEnabled(MsBuildProject project)
            => project.GetPropertyValue("EnableCentralPackageVersions") != "false";

        private static string GetCentralPackagesFile(MsBuildProject project)
            => project.GetPropertyValue("CentralPackagesFile");

        private static void AddPackageReferencesToProject(MsBuildProject project, MsBuildProject? centralPackagesFile, IEnumerable<PackageReference> packageReferences)
        {
            if (packageReferences.Any())
            {
                var itemGroup = project.GetItemGroupWithItemOfTypeOrCreateNew(PackageReferenceTypeTag);
                var centralPackageItemGroup = centralPackagesFile?.GetItemGroupWithItemOfTypeOrCreateNew(PackageReferenceTypeTag);
                foreach (var packageReference in packageReferences)
                {
                    AddPackageReference(itemGroup, centralPackageItemGroup, packageReference);
                }
            }
        }

        private static void AddPackageReference(ProjectItemGroupElement itemGroup, ProjectItemGroupElement? centralPackageItemGroup, PackageReference packageReference)
        {
            var item = itemGroup.AddItem(PackageReferenceTypeTag, packageReference.Name);

            if (centralPackageItemGroup is { })
            {
                var centralPackageItem = centralPackageItemGroup.ContainingProject.CreateItemElement(PackageReferenceTypeTag);
                centralPackageItem.Update = packageReference.Name;
                centralPackageItemGroup.AppendChild(centralPackageItem);
                centralPackageItem.AddMetadataAsAttribute(VersionMetadataAttribute, packageReference.Version);
            }
            else
            {
                item.AddMetadataAsAttribute(VersionMetadataAttribute, packageReference.Version);
            }

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

        private static void AddAssetsListMetadataToPackageReference(
            ProjectItemElement item,
            string attributeName,
            DependencyAssets? assetList)
        {
            if (assetList is { })
            {
                item.AddMetadataAsAttribute(attributeName, MapDependencyAssetsToString(assetList));
            }
        }

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
    }
}