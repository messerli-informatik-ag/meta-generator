using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Messerli.FileManipulatorAbstractions
{
    public sealed class GlobalJsonModificationBuilder
    {
        private readonly IImmutableList<MsBuildSdk> _msBuildSdksToAdd;

        public GlobalJsonModificationBuilder()
            : this(ImmutableList<MsBuildSdk>.Empty)
        {
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305: FieldNamesMustNotUseHungarianNotation", Justification = "Not hungarian notation")]
        private GlobalJsonModificationBuilder(IImmutableList<MsBuildSdk> msBuildSdksToAdd)
        {
            _msBuildSdksToAdd = msBuildSdksToAdd;
        }

        [Pure]
        public GlobalJsonModificationBuilder AddMsBuildSdk(MsBuildSdk sdk)
            => ShallowClone(sdksToAdd: _msBuildSdksToAdd.Add(sdk));

        [Pure]
        public GlobalJsonModification Build()
            => new GlobalJsonModification(_msBuildSdksToAdd);

        private GlobalJsonModificationBuilder ShallowClone(
            IImmutableList<MsBuildSdk>? sdksToAdd = null)
            => new GlobalJsonModificationBuilder(sdksToAdd ?? _msBuildSdksToAdd);
    }
}
