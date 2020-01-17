using System.IO;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface ITemplateLoader
    {
        string GetTemplate(string templateName);

        Stream? GetTemplateStream(string templateName);
    }
}