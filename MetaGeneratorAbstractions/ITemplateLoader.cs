using System.Collections.Generic;
using System.IO;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface ITemplateLoader
    {
        string GetTemplate(string templateName);

        IEnumerable<Template> GetTemplatesFromGlob(string glob);

        Stream? GetTemplateStream(string templateName);
    }
}