using System;
using System.Diagnostics.Contracts;

namespace Messerli.FileManipulatorAbstractions.Project
{
    public sealed class PackageReferenceBuilder
    {
        private readonly string? _name;

        private readonly string? _version;

        private readonly DependencyAssets? _privateAssets;

        private readonly DependencyAssets? _excludeAssets;

        private readonly DependencyAssets? _includeAssets;

        public PackageReferenceBuilder()
        {
        }

        private PackageReferenceBuilder(
            string? name,
            string? version,
            DependencyAssets? privateAssets,
            DependencyAssets? excludeAssets,
            DependencyAssets? includeAssets)
        {
            _name = name;
            _version = version;
            _privateAssets = privateAssets;
            _excludeAssets = excludeAssets;
            _includeAssets = includeAssets;
        }

        [Pure]
        public PackageReference Build()
            => new PackageReference(
                _name ?? throw new InvalidOperationException($"{nameof(Name)} is required, did you forget to call .{nameof(Name)}()?"),
                _version ?? throw new InvalidOperationException($"{nameof(Version)} is required, did you forget to call .{nameof(Version)}()?"),
                _privateAssets,
                _excludeAssets,
                _includeAssets);

        [Pure]
        public PackageReferenceBuilder Name(string name)
            => ShallowClone(name: name);

        [Pure]
        public PackageReferenceBuilder Version(string version)
            => ShallowClone(version: version);

        [Pure]
        public PackageReferenceBuilder PrivateAssets(DependencyAssets privateAssets)
            => ShallowClone(privateAssets: privateAssets);

        [Pure]
        public PackageReferenceBuilder IncludeAssets(DependencyAssets includeAssets)
            => ShallowClone(includeAssets: includeAssets);

        [Pure]
        public PackageReferenceBuilder ExcludeAssets(DependencyAssets excludeAssets)
            => ShallowClone(excludeAssets: excludeAssets);

        private PackageReferenceBuilder ShallowClone(
            string? name = null,
            string? version = null,
            DependencyAssets? privateAssets = null,
            DependencyAssets? excludeAssets = null,
            DependencyAssets? includeAssets = null)
            => new PackageReferenceBuilder(
                name ?? _name,
                version ?? _version,
                privateAssets ?? _privateAssets,
                excludeAssets ?? _excludeAssets,
                includeAssets ?? _includeAssets);
    }
}