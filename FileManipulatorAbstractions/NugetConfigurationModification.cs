using System.Collections.Generic;

namespace Messerli.FileManipulatorAbstractions
{
    public delegate void NugetPackageSourceManipulationAction(string filePath);

    /// <summary>
    /// This type can be constructed using a <see cref="NugetConfigurationModificationBuilder"/>.
    /// </summary>
    public sealed class NugetConfigurationModification
    {
        internal NugetConfigurationModification(IEnumerable<NugetPackageSourceManipulationAction> packageSourceManipulations)
        {
            NugetPackageSourceManipulations = packageSourceManipulations;
        }

        public IEnumerable<NugetPackageSourceManipulationAction> NugetPackageSourceManipulations { get; }
    }
}