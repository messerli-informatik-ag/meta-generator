using System.IO;
using System.Linq;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.BackbonePluginTemplatePlugin
{
    internal static class InputValidations
    {
        public static IValidation AnySolutionExists
            => SimpleValidation.Create(path => AnySolutionExistsInPath(path), "No Solution (.sln) found at given location.");

        private static bool AnySolutionExistsInPath(string path)
            => Directory.Exists(path) && Directory.GetFiles(path, "*.sln").Any();
    }
}
