using System.IO;
using System.Threading.Tasks;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Parser;

namespace Messerli.VsSolution
{
    public class SolutionLoader : ISolutionLoader
    {
        private readonly SolutionParser _parser;
        private readonly SolutionWriter _writer;

        public SolutionLoader()
        {
            _parser = SolutionParser.Create();
            _writer = SolutionWriter.Create();
        }

        public async Task<Solution> Load(string solutionPath)
        {
            return _parser.Parse(await File.ReadAllTextAsync(solutionPath));
        }

        public async Task Store(string solutionPath, Solution solution)
        {
            await File.WriteAllTextAsync(solutionPath, _writer.Serialize(solution));
        }
    }
}
