using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Messerli.FileManipulatorAbstractions
{
    public sealed class GlobalJsonModification
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305: FieldNamesMustNotUseHungarianNotation", Justification = "Not hungarian notation")]
        public GlobalJsonModification(IEnumerable<MsBuildSdk> msBuildSdksToAdd)
        {
            MsBuildSdksToAdd = msBuildSdksToAdd;
        }

        public IEnumerable<MsBuildSdk> MsBuildSdksToAdd { get; }
    }
}
