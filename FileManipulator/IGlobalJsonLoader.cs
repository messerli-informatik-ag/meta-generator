using System.Threading.Tasks;

namespace Messerli.FileManipulator
{
    public interface IGlobalJsonLoader
    {
        Task<GlobalJson> Load(string path);

        Task Store(string path, GlobalJson globalJson);
    }
}
