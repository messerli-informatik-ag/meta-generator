using System.Threading.Tasks;

namespace Messerli.FileManipulatorAbstractions
{
    public interface IGlobalJsonManipulator
    {
        /// <exception cref="GlobalJsonManipulationException">Thrown when a <see cref="MsBuildSdk"/> already exists with a conflicting version.</exception>
        /// <exception cref="GlobalJsonManipulationException">Thrown when the config file is malformed.</exception>
        Task ModifyGlobalJson(string filePath, GlobalJsonModification modification);
    }
}
