using System.Collections.Generic;

namespace Messerli.FileManipulatorAbstractions.Project
{
    /// <summary>
    /// This class can be constructed using a <see cref="ProjectModificationBuilder"/>.
    /// </summary>
    public sealed class ProjectModification
    {
        internal ProjectModification(IEnumerable<PackageReference> packageReferencesToAdd)
        {
            PackageReferencesToAdd = packageReferencesToAdd;
        }

        public IEnumerable<PackageReference> PackageReferencesToAdd { get; }
    }
}