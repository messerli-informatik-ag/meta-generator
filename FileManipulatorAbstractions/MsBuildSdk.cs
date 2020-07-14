namespace Messerli.FileManipulatorAbstractions
{
    public class MsBuildSdk
    {
        public MsBuildSdk(string nugetPackageId, string version)
        {
            NuGetPackageId = nugetPackageId;
            Version = version;
        }

        public string NuGetPackageId { get; }

        public string Version { get; }
    }
}
