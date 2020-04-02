using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace Messerli.FileManipulatorAbstractions.Project
{
    public sealed class ProjectModificationBuilder
    {
        private readonly IImmutableList<PackageReference> _packageReferencesToAdd;

        public ProjectModificationBuilder()
            : this(ImmutableList<PackageReference>.Empty)
        {
        }

        private ProjectModificationBuilder(IImmutableList<PackageReference> packageReferencesToAdd)
        {
            _packageReferencesToAdd = packageReferencesToAdd;
        }

        [Pure]
        public ProjectModification Build()
            => new ProjectModification(_packageReferencesToAdd);

        [Pure]
        public ProjectModificationBuilder AddPackageReference(PackageReference packageReference)
            => ShallowClone(packageReferencesToAdd: _packageReferencesToAdd.Add(packageReference));

        [Pure]
        public ProjectModificationBuilder AddPackageReferences(IEnumerable<PackageReference> packageReference)
            => ShallowClone(packageReferencesToAdd: _packageReferencesToAdd.AddRange(packageReference));

        private ProjectModificationBuilder ShallowClone(
            IImmutableList<PackageReference>? packageReferencesToAdd = null)
            => new ProjectModificationBuilder(
                packageReferencesToAdd ?? _packageReferencesToAdd);
    }
}