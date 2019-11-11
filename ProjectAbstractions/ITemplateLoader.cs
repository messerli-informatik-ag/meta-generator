using System.Reflection;

namespace Messerli.ProjectAbstractions
{
    public interface ITemplateLoader
    {
        string GetTemplate(string templateName);
    }
}