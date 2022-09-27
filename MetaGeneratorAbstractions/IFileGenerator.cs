using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Messerli.MetaGeneratorAbstractions;

public interface IFileGenerator
{
    Task FromTemplate(string templatename, string destinationPath);

    Task FromTemplate(string templatename, string destinationPath, Encoding encoding);

    /// <summary>
    /// Embedded resources need to annotated with LogicalName="%(Identity)" in order to
    /// preserve the directory information needed for this method.
    /// <param name="glob">The glob pattern matching the embedded resources used as templates</param>
    /// <param name="destinationDirectory">The root directory to copy the matched templates to</param>
    /// </summary>
    Task FromTemplateGlob(string glob, string destinationDirectory);

    /// <summary>
    /// Embedded resources need to annotated with LogicalName="%(Identity)" in order to
    /// preserve the directory information needed for this method.
    /// <param name="glob">The glob pattern matching the embedded resources used as templates</param>
    /// <param name="destinationDirectory">The root directory to copy the matched templates to</param>
    /// <param name="fileNameTemplateValues">A map used to replaced template file name parts annotated with { and }</param>
    /// </summary>
    Task FromTemplateGlob(string glob, string destinationDirectory, IDictionary<string, string> fileNameTemplateValues);

    /// <summary>
    /// Embedded resources need to annotated with LogicalName="%(Identity)" in order to
    /// preserve the directory information needed for this method.
    /// <param name="glob">The glob pattern matching the embedded resources used as templates</param>
    /// <param name="destinationDirectory">The root directory to copy the matched templates to</param>
    /// <param name="fileNameTemplateValues">A map used to replaced template file name parts annotated with { and }</param>
    /// <param name="encoding">The encoding for the generated files</param>
    /// </summary>
    Task FromTemplateGlob(string glob, string destinationDirectory, IDictionary<string, string> fileNameTemplateValues, Encoding encoding);
}
