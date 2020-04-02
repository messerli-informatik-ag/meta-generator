using Messerli.FileManipulatorAbstractions.Project;

namespace Messerli.FileManipulator.Project.MsBuild
{
    internal interface IProjectManipulator
    {
        void ManipulateProject(string projectFilePath, ProjectModification modification);
    }
}