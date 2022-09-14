using System.Threading.Tasks;
using Messerli.VsSolution.Model;

namespace Messerli.VsSolution;

public interface ISolutionLoader
{
    Task<Solution> Load(string solutionPath);

    Task Store(string solutionPath, Solution solution);
}
