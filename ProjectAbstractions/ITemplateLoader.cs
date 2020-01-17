using System.IO;

namespace Messerli.ProjectGenerator
{
    public interface ITemplateLoader
    {
        string GetTemplate(string templateName);

        Stream? GetTemplateStream(string templateName);
    }
}