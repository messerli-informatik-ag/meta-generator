using System.Threading.Tasks;

namespace Messerli.ProjectAbstractions
{
    public interface IFileManipulator
    {
        Task AppendTemplate(string templatename, string filePath);
    }
}