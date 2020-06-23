using System.Collections.Generic;

namespace Messerli.FileManipulatorAbstractions.Project
{
    /// <summary>
    /// This class can be constructed using a <see cref="ProjectModificationBuilder"/>.
    /// </summary>
    public sealed class ProjectModification
    {
        internal ProjectModification(IEnumerable<PackageReference> packageReferencesToAdd, IEnumerable<string> sdksToAdd)
        {
            PackageReferencesToAdd = packageReferencesToAdd;
            SdksToAdd = sdksToAdd;
        }

        public IEnumerable<PackageReference> PackageReferencesToAdd { get; }

        public IEnumerable<string> SdksToAdd { get; }
    }
}