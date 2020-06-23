using System;

namespace Messerli.FileManipulatorAbstractions.Project
{
    public sealed class ConflictingPackageReferenceException : Exception
    {
        public ConflictingPackageReferenceException(PackageReference packageReference, string currentVersion)
        {
            PackageReference = packageReference;
            CurrentVersion = currentVersion;
        }

        public PackageReference PackageReference { get; }

        public string CurrentVersion { get; }

        public override string Message => $"Package reference '{PackageReference.Name} already exists with conflicting version {CurrentVersion}";
    }
}