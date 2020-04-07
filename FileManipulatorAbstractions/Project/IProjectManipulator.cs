using System.Threading.Tasks;

namespace Messerli.FileManipulatorAbstractions.Project
{
    public interface IProjectManipulator
    {
        /// <exception cref="ProjectManipulationException">Thrown when <paramref name="projectFilePath"/> does not exist. See the inner exception for details.</exception>
        /// <exception cref="ProjectManipulationException">Thrown when the project file could not be loaded (e.g. syntax error).</exception>
        Task ManipulateProject(string projectFilePath, ProjectModification modification);
    }
}