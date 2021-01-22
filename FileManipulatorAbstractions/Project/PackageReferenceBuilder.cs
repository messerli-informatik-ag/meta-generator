using System;
using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Messerli.FileManipulatorAbstractions.Project
{
    public sealed class PackageReferenceBuilder
    {
        private readonly Option<string> _name;

        private readonly Option<string> _version;

        private readonly Option<DependencyAssets> _privateAssets;

        private readonly Option<DependencyAssets> _excludeAssets;

        private readonly Option<DependencyAssets> _includeAssets;

        public PackageReferenceBuilder()
        {
        }

        private PackageReferenceBuilder(
            Option<string> name,
            Option<string> version,
            Option<DependencyAssets> privateAssets,
            Option<DependencyAssets> excludeAssets,
            Option<DependencyAssets> includeAssets)
        {
            _name = name;
            _version = version;
            _privateAssets = privateAssets;
            _excludeAssets = excludeAssets;
            _includeAssets = includeAssets;
        }

        [Pure]
        public PackageReference Build()
            => new(
                _name.GetOrElse(() => throw new InvalidOperationException($"{nameof(Name)} is required, did you forget to call .{nameof(Name)}()?")),
                _version.GetOrElse(() => throw new InvalidOperationException($"{nameof(Version)} is required, did you forget to call .{nameof(Version)}()?")),
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
            Option<string> name = default,
            Option<string> version = default,
            Option<DependencyAssets> privateAssets = default,
            Option<DependencyAssets> excludeAssets = default,
            Option<DependencyAssets> includeAssets = default)
            => new(
                name.OrElse(_name),
                version.OrElse(_version),
                privateAssets.OrElse(_privateAssets),
                excludeAssets.OrElse(_excludeAssets),
                includeAssets.OrElse(_includeAssets));
    }
}
