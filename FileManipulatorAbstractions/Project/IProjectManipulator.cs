using System.IO;
using System.Threading.Tasks;

namespace Messerli.FileManipulatorAbstractions.Project
{
    public interface IProjectManipulator
    {
        /// <exception cref="FileNotFoundException">Thrown when <paramref name="projectFilePath"/> does not exist.</exception>
        Task ManipulateProject(string projectFilePath, ProjectModification modification);
    }
}