using System.Collections.Generic;

namespace Messerli.FileManipulatorAbstractions
{
    public sealed class GlobalJsonModification
    {
        public GlobalJsonModification(IEnumerable<MsBuildSdk> sdksToAdd)
        {
            SdksToAdd = sdksToAdd;
        }

        public IEnumerable<MsBuildSdk> SdksToAdd { get; }
    }
}
