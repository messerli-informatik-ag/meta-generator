using System.IO;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGeneratorProjectPlugin
{
    internal static class InputValidations
    {
        private const string MetaGeneratorSolution = "MetaGenerator.sln";

        public static IValidation MetaGeneratorSolutionExists
            => SimpleValidation.Create(MetaGeneratorSolutionInPath, "No MetaGenerator.sln found at given location.");

        private static bool MetaGeneratorSolutionInPath(string path)
            => File.Exists(AppendSolution(path));

        private static string AppendSolution(string path)
            => Path.Combine(path, MetaGeneratorSolution);
    }
}
