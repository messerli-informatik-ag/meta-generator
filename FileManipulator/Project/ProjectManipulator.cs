using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions.Project;
using Messerli.FileManipulatorAbstractions.Project.AssetList;
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
            IAssetListVariant? assetList)
        {
            if (assetList is { })
            {
                item.AddMetadataAsAttribute(attributeName, MapAssetListToString(assetList));
            }
        }

        private static string MapAssetListToString(IAssetListVariant assetList)
            => assetList switch
            {
                All _ => "all",
                None _ => "none",
                List list => string.Join(ListSeparator, list.Assets.Select(MapAssetNameToString)),
                _ => throw new InvalidOperationException($"Enum variant {assetList.GetType().Name} is not supported"),
            };

        private static string MapAssetNameToString(AssetName assetName)
            => assetName switch
            {
                AssetName.Compile => "compile",
                AssetName.Runtime => "runtime",
                AssetName.ContentFiles => "contentFiles",
                AssetName.Build => "build",
                AssetName.BuildMultiTargeting => "buildMultitargeting",
                AssetName.BuildTransitive => "buildTransitive",
                AssetName.Analyzers => "analyzers",
                AssetName.Native => "native",
                _ => throw new InvalidOperationException($"Enum variant {assetName} is not supported"),
            };
    }
}