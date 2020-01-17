using System.Threading.Tasks;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface IFileManipulator
    {
        Task AppendTemplate(string templatename, string filePath);

        Task AddProjectToSolution(string? solutionFolder, string projectName, string projectPath, string solutionPath);
    }
}