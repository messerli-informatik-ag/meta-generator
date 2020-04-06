using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace Messerli.FileManipulatorAbstractions
{
    public sealed class NugetConfigurationModificationBuilder
    {
        private readonly INugetPackageSourceManipulator _nugetPackageSourceManipulator;
        private readonly IImmutableList<NugetPackageSourceManipulationAction> _packageSourceManipulations;

        public NugetConfigurationModificationBuilder(INugetPackageSourceManipulator nugetPackageSourceManipulator)
            : this(
                nugetPackageSourceManipulator,
                ImmutableList<NugetPackageSourceManipulationAction>.Empty)
        {
        }

        private NugetConfigurationModificationBuilder(
            INugetPackageSourceManipulator nugetPackageSourceManipulator,
            IImmutableList<NugetPackageSourceManipulationAction> packageSourceManipulations)
        {
            _nugetPackageSourceManipulator = nugetPackageSourceManipulator;
            _packageSourceManipulations = packageSourceManipulations;
        }

        [Pure]
        public NugetConfigurationModificationBuilder AddPackageSource(NugetPackageSource packageSource)
            => ShallowClone(_packageSourceManipulations.Add(filePath =>
                _nugetPackageSourceManipulator.Add(filePath, packageSource)));

        [Pure]
        public NugetConfigurationModificationBuilder UpdatePackageSource(NugetPackageSource packageSource)
            => ShallowClone(_packageSourceManipulations.Add(filePath =>
                _nugetPackageSourceManipulator.Update(filePath, packageSource)));

        [Pure]
        public NugetConfigurationModificationBuilder RemovePackageSource(string packageSourceName)
            => ShallowClone(_packageSourceManipulations.Add(filePath =>
                _nugetPackageSourceManipulator.Remove(filePath, packageSourceName)));

        [Pure]
        public NugetConfigurationModificationBuilder EnablePackageSource(string packageSourceName)
            => ShallowClone(_packageSourceManipulations.Add(filePath =>
                _nugetPackageSourceManipulator.Enable(filePath, packageSourceName)));

        [Pure]
        public NugetConfigurationModificationBuilder DisablePackageSource(string packageSourceName)
            => ShallowClone(_packageSourceManipulations.Add(filePath =>
                _nugetPackageSourceManipulator.Disable(filePath, packageSourceName)));

        [Pure]
        public NugetConfigurationModification Build()
            => new NugetConfigurationModification(_packageSourceManipulations);

        private NugetConfigurationModificationBuilder ShallowClone(
            IImmutableList<NugetPackageSourceManipulationAction>? packageSourceManipulations = null)
            => new NugetConfigurationModificationBuilder(
                _nugetPackageSourceManipulator,
                packageSourceManipulations ?? _packageSourceManipulations);
    }
}