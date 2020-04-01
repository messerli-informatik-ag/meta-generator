using System.Collections.Immutable;

namespace Messerli.FileManipulatorAbstractions
{
    public sealed class GlobalJsonModificationBuilder
    {
        private readonly IImmutableList<MsBuildSdk> _sdksToAdd;

        public GlobalJsonModificationBuilder()
            : this(ImmutableList<MsBuildSdk>.Empty)
        {
        }

        private GlobalJsonModificationBuilder(IImmutableList<MsBuildSdk> sdksToAdd)
        {
            _sdksToAdd = sdksToAdd;
        }

        public GlobalJsonModificationBuilder AddSdk(MsBuildSdk sdk)
            => ShallowClone(sdkList: _sdksToAdd.Add(sdk));

        public GlobalJsonModification Build(string path)
            => new GlobalJsonModification(_sdksToAdd);

        private GlobalJsonModificationBuilder ShallowClone(
            IImmutableList<MsBuildSdk>? sdkList = null)
            => new GlobalJsonModificationBuilder(sdkList ?? _sdksToAdd);
    }
}
