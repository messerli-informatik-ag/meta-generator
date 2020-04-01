using System.Threading.Tasks;

namespace Messerli.FileManipulatorAbstractions
{
    public interface IGlobalJsonManipulator
    {
        Task ModifyGlobalJson(string path, GlobalJsonModification modification);
    }
}
