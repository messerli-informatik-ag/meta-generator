using Messerli.FileManipulatorAbstractions.Project.AssetList;

namespace Messerli.FileManipulatorAbstractions.Project
{
    /// <summary>
    /// This class can be constructed using a <see cref="PackageReferenceBuilder"/>.
    /// </summary>
    public sealed class PackageReference
    {
        internal PackageReference(
            string name,
            string version,
            IAssetListVariant? privateAssets = null,
            IAssetListVariant? excludeAssets = null,
            IAssetListVariant? includeAssets = null)
        {
            Name = name;
            Version = version;
            PrivateAssets = privateAssets;
            ExcludeAssets = excludeAssets;
            IncludeAssets = includeAssets;
        }

        public string Name { get; }

        public string Version { get; }

        public IAssetListVariant? PrivateAssets { get; }

        public IAssetListVariant? ExcludeAssets { get; }

        public IAssetListVariant? IncludeAssets { get; }
    }
}