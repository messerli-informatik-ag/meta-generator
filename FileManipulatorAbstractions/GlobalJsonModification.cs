using System.Collections.Generic;

namespace Messerli.FileManipulatorAbstractions
{
    public sealed class GlobalJsonModification
    {
        public GlobalJsonModification(string path, List<MsBuildSdk> msbuildSdk)
        {
            Path = path;
            SdkList = msbuildSdk;
        }

        public List<MsBuildSdk> SdkList { get; }

        public string Path { get; }
    }
}
