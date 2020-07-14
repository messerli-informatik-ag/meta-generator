using System.Collections.Generic;
using System.IO;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface ITemplateLoader
    {
        string GetTemplate(string templateName);

        /// <summary>
        /// Embedded resources need to annotated with LogicalName="%(Identity)" in order to
        /// preserve the directory information needed for this method.
        /// <param name="glob">The glob pattern matching the embedded resources used as templates</param>
        /// </summary>
        IEnumerable<Template> GetTemplatesFromGlob(string glob);

        Stream? GetTemplateStream(string templateName);
    }
}