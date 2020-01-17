using System.Text;
using System.Threading.Tasks;

namespace Messerli.MetaGeneratorAbstractions
{
    public interface IFileGenerator
    {
        Task FromTemplate(string templatename, string destinationPath, Encoding encoding);
    }
}
