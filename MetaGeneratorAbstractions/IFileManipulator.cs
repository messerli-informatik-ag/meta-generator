using System.Collections.Generic;
using System.Threading.Tasks;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface IFileManipulator
    {
        Task AppendTemplate(string templatename, string filePath);

        Task AddProjectsToSolution(SolutionInfo solutionInfo, IEnumerable<ProjectInfo> projectInfo);
    }
}