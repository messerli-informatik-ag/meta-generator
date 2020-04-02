using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface IFileGenerator
    {
        Task FromTemplate(string templatename, string destinationPath);

        Task FromTemplate(string templatename, string destinationPath, Encoding encoding);

        Task FromTemplateGlob(string glob);

        Task FromTemplateGlob(string glob, IDictionary<string, string> fileNameTemplateValues);

        Task FromTemplateGlob(string glob, IDictionary<string, string> fileNameTemplateValues, Encoding encoding);
    }
}
