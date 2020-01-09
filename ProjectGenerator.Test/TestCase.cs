using Messerli.VsSolution;
using Messerli.VsSolution.Model;
using Xunit;

namespace Messerli.ProjectGenerator.Test
{
    public class TestCase
    {
        [Fact]
        public void ParseSolution()
        {
            var solutionLoader = new SolutionLoader();

            var solution = solutionLoader.Load(@"C:\Repositories\ProjectGenerator\ProjectGenerator.sln").Result;

            solution.AddProject("Name", "NewPath", ProjectType.DotNetStandard);

            solutionLoader.Store(@"C:\Repositories\ProjectGenerator\Result.sln", solution).Wait();
        }
    }
}