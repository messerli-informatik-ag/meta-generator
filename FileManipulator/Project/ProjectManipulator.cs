using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions.Project;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project
{
    public sealed class ProjectManipulator : IProjectManipulator
    {
        private const string PackageReferenceTypeTag = "PackageReference";
        private const string VersionMetadataAttribute = "Version";
        private const string PrivateAssetsMetadataAttribute = "PrivateAssets";
        private const string IncludeAssetsMetadataAttribute = "IncludeAssets";
        private const string ExcludeAssetsMetadataAttribute = "ExcludeAssets";
        private const char ListSeparator = ';';

        private readonly IMicrosoftBuildAssemblyLoader _microsoftBuildAssemblyLoader;

        public ProjectManipulator(IMicrosoftBuildAssemblyLoader microsoftBuildAssemblyLoader)
        {
            _microsoftBuildAssemblyLoader = microsoftBuildAssemblyLoader;
        }

        public Task ManipulateProject(string projectFilePath, ProjectModification modification)
        {
            _microsoftBuildAssemblyLoader.LoadMicrosoftBuildIfNecessary();
            ManipulateProjectInternal(projectFilePath, modification);
            return Task.CompletedTask;
        }

        private static void ManipulateProjectInternal(string projectFilePath, ProjectModification modification)
        {
            using var projectCollection = new ProjectCollection();
            var project = OpenProject(projectFilePath, projectCollection);
            AddPackageReferencesToProject(project, modification.PackageReferencesToAdd);
            project.Save();
        }

        private static MsBuildProject OpenProject(string projectFilePath, ProjectCollection projectCollection)
        {
            var projectRootElement = ProjectRootElement.Open(projectFilePath, projectCollection, preserveFormatting: true);
            return new MsBuildProject(projectRootElement);
        }

        private static void AddPackageReferencesToProject(MsBuildProject project, IEnumerable<PackageReference> packageReferences)
        {
            if (packageReferences.Any())
            {
                var itemGroup = project.GetItemGroupWithItemOfTypeOrCreateNew(PackageReferenceTypeTag);

                foreach (var packageReference in packageReferences)
                {
                    AddPackageReference(itemGroup, packageReference);
                }
            }
        }

        private static void AddPackageReference(ProjectItemGroupElement itemGroup, PackageReference packageReference)
        {
            var item = itemGroup.AddItem(PackageReferenceTypeTag, packageReference.Name);

            item.AddMetadataAsAttribute(VersionMetadataAttribute, packageReference.Version);

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