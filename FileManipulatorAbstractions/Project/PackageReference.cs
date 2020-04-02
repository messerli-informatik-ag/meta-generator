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
            DependencyAssets? privateAssets = null,
            DependencyAssets? excludeAssets = null,
            DependencyAssets? includeAssets = null)
        {
            Name = name;
            Version = version;
            PrivateAssets = privateAssets;
            ExcludeAssets = excludeAssets;
            IncludeAssets = includeAssets;
        }

        public string Name { get; }

        public string Version { get; }

        public DependencyAssets? PrivateAssets { get; }

        public DependencyAssets? ExcludeAssets { get; }

        public DependencyAssets? IncludeAssets { get; }
    }
}