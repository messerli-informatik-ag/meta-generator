using System.Threading.Tasks;

namespace Messerli.FileManipulatorAbstractions
{
    public interface IGlobalJsonManipulator
    {
        /// <exception cref="GlobalJsonManipulationException">Thrown when a <see cref="MsBuildSdk"/> already exists with a conflicting version.</exception>
        Task ModifyGlobalJson(string path, GlobalJsonModification modification);
    }
}
