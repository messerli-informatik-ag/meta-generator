using System.Threading.Tasks;

namespace Messerli.ProjectAbstractions
{
    public interface IFileGenerator
    {
        Task FromTemplate(string templatename, string destinationPath);
    }
}
