using Funcky.Monads;

namespace Messerli.FileManipulatorAbstractions.Project;

/// <summary>
/// This class can be constructed using a <see cref="PackageReferenceBuilder"/>.
/// </summary>
public sealed class PackageReference
{
    internal PackageReference(
        string name,
        string version,
        Option<DependencyAssets> privateAssets = default,
        Option<DependencyAssets> excludeAssets = default,
        Option<DependencyAssets> includeAssets = default)
    {
        Name = name;
        Version = version;
        PrivateAssets = privateAssets;
        ExcludeAssets = excludeAssets;
        IncludeAssets = includeAssets;
    }

    public string Name { get; }

    public string Version { get; }

    public Option<DependencyAssets> PrivateAssets { get; }

    public Option<DependencyAssets> ExcludeAssets { get; }

    public Option<DependencyAssets> IncludeAssets { get; }
}
