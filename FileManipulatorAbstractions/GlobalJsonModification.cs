using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Messerli.FileManipulatorAbstractions
{
    /// <summary>
    /// This type can be constructed using a <see cref="GlobalJsonModificationBuilder"/>.
    /// </summary>
    public sealed class GlobalJsonModification
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305: FieldNamesMustNotUseHungarianNotation", Justification = "Not hungarian notation")]
        internal GlobalJsonModification(IEnumerable<MsBuildSdk> msBuildSdksToAdd)
        {
            MsBuildSdksToAdd = msBuildSdksToAdd;
        }

        public IEnumerable<MsBuildSdk> MsBuildSdksToAdd { get; }
    }
}
