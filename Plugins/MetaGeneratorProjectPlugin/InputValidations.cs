using System.IO;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGeneratorProjectPlugin
{
    internal static class InputValidations
    {
        public static IValidation MetaGeneratorSolutionExists
            => new SimpleValidation(path => File.Exists(Path.Combine(path, "MetaGenerator.sln")), "No MetaGenerator.sln found at given location.");
    }
}
