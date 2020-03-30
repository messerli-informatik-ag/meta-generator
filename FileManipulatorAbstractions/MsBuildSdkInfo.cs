using System.Collections.Generic;

namespace Messerli.FileManipulatorAbstractions
{
    public sealed class MsBuildSdkInfo
    {
        public MsBuildSdkInfo(string path, List<MsBuildSdk> msbuildSdk)
        {
            Path = path;
            SdkList = msbuildSdk;
        }

        public List<MsBuildSdk> SdkList { get; }

        public string Path { get; }
    }
}
