using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions.Project;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
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
        private const string CentralPackageVersionsSdk = "Microsoft.Build.CentralPackageVersions";

        private readonly IMicrosoftBuildAssemblyLoader _microsoftBuildAssemblyLoader;

        public ProjectManipulator(IMicrosoftBuildAssemblyLoader microsoftBuildAssemblyLoader)
        {
            _microsoftBuildAssemblyLoader = microsoftBuildAssemblyLoader;
        }

        public Task ManipulateProject(string projectFilePath, ProjectModification modification)
        {
            _microsoftBuildAssemblyLoader.LoadMicrosoftBuildIfNecessary();
            WrapExceptions(projectFilePath, () => ManipulateProjectInternal(projectFilePath, modification));
            return Task.CompletedTask;
        }

        private static void WrapExceptions(string projectFilePath, Action action)
        {
            try
            {
                action();
            }
            catch (InvalidProjectFileException exception) when (exception.InnerException is FileNotFoundException)
            {
                throw new ProjectManipulationException(exception.InnerException, projectFilePath);
            }
            catch (Exception exception)
            {
                throw new ProjectManipulationException(exception, projectFilePath);
            }
        }

        private static void ManipulateProjectInternal(string projectFilePath, ProjectModification modification)
        {
            using var projectCollection = new ProjectCollection();
            var project = OpenProject(projectFilePath, projectCollection);
            var centralPackagesFile = UseCentralPackageVersionsSdk(project) && HasCentralPackageVersionsEnabled(project)
                ? OpenProject(GetCentralPackagesFile(project), projectCollection)
                : null;
            AddSdksToProject(project, modification.SdksToAdd);
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

        private static void AddSdksToProject(MsBuildProject project, IEnumerable<string> sdksToAdd)
        {
            var existingSdks = ParseSdkList(project.Xml.Sdk);
            var sdks = existingSdks
                .Concat(sdksToAdd)
                .Distinct();
            project.Xml.Sdk = string.Join($"{ListSeparator} ", sdks);
        }

        private static IEnumerable<string> ParseSdkList(string sdkList)
            => sdkList == string.Empty
                ? new string[0]
                : sdkList.Split(ListSeparator).Select(s => s.Trim());

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