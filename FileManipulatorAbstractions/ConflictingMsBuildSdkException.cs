using System;

namespace Messerli.FileManipulatorAbstractions
{
    public sealed class ConflictingMsBuildSdkException : Exception
    {
        public ConflictingMsBuildSdkException(MsBuildSdk sdk, string currentVersion)
        {
            Sdk = sdk;
            CurrentVersion = currentVersion;
        }

        public MsBuildSdk Sdk { get; }

        public string CurrentVersion { get; }

        public override string Message => $"MSBuild SDK '{Sdk.NuGetPackageId}' already exists with conflicting version '{CurrentVersion}'";
    }
}