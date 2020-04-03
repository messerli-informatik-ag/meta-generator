namespace Messerli.FileManipulator.Project.MsBuild
{
    internal abstract class PackageReferenceConflictResult
    {
        public sealed class NoExisting : PackageReferenceConflictResult
        {
        }

        public sealed class ExistingIsCompatible : PackageReferenceConflictResult
        {
        }

        public sealed class ExistingIsIncompatible : PackageReferenceConflictResult
        {
            public ExistingIsIncompatible(string version)
            {
                Version = version;
            }

            public string Version { get; }
        }
    }
}