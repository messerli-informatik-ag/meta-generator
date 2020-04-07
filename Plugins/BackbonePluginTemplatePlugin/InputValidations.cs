using System.IO;
using System.Linq;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.BackbonePluginTemplatePlugin
{
    internal static class InputValidations
    {
        public static IValidation AnySolutionExists
            => new SimpleValidation(path => Directory.Exists(path) && Directory.GetFiles(path, "*.sln").Any(), "No Solution (.sln) found at given location.");
    }
}
