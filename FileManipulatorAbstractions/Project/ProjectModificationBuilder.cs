using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace Messerli.FileManipulatorAbstractions.Project
{
    public sealed class ProjectModificationBuilder
    {
        private readonly IImmutableList<PackageReference> _packageReferencesToAdd;

        private readonly IImmutableList<string> _sdksToAdd;

        public ProjectModificationBuilder()
            : this(ImmutableList<PackageReference>.Empty, ImmutableList<string>.Empty)
        {
        }

        private ProjectModificationBuilder(
            IImmutableList<PackageReference> packageReferencesToAdd,
            IImmutableList<string> sdksToAdd)
        {
            _packageReferencesToAdd = packageReferencesToAdd;
            _sdksToAdd = sdksToAdd;
        }

        [Pure]
        public ProjectModification Build()
            => new ProjectModification(_packageReferencesToAdd, _sdksToAdd);

        [Pure]
        public ProjectModificationBuilder AddPackageReference(PackageReference packageReference)
            => ShallowClone(packageReferencesToAdd: _packageReferencesToAdd.Add(packageReference));

        [Pure]
        public ProjectModificationBuilder AddSdk(string sdk)
            => ShallowClone(sdksToAdd: _sdksToAdd.Add(sdk));

        [Pure]
        public ProjectModificationBuilder AddPackageReferences(IEnumerable<PackageReference> packageReference)
            => ShallowClone(packageReferencesToAdd: _packageReferencesToAdd.AddRange(packageReference));

        private ProjectModificationBuilder ShallowClone(
            IImmutableList<PackageReference>? packageReferencesToAdd = null,
            IImmutableList<string>? sdksToAdd = null)
            => new ProjectModificationBuilder(
                packageReferencesToAdd ?? _packageReferencesToAdd,
                sdksToAdd ?? _sdksToAdd);
    }
}