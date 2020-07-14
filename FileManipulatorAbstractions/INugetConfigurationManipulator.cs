using System.Threading.Tasks;

namespace Messerli.FileManipulatorAbstractions
{
    public interface INugetConfigurationManipulator
    {
        Task ModifyNugetConfiguration(string filePath, NugetConfigurationModification modification);
    }
}