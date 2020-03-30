using System.Threading.Tasks;

namespace Messerli.FileManipulatorAbstractions
{
    public interface IGlobalJsonManipulator
    {
        Task AddMsBuildSdkToGlobalJson(MsBuildSdkInfo sdkInfo);
    }
}
