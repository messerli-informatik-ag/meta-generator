using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions;

namespace Messerli.FileManipulator;

public sealed class NugetConfigurationManipulator : INugetConfigurationManipulator
{
    public Task ModifyNugetConfiguration(string filePath, NugetConfigurationModification modification)
    {
        foreach (var manipulation in modification.NugetPackageSourceManipulations)
        {
            manipulation(filePath);
        }

        return Task.CompletedTask;
    }
}
