using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions;

namespace Messerli.FileManipulator
{
    public sealed class GlobalJsonManipulator : IGlobalJsonManipulator
    {
        private readonly IGlobalJsonLoader _globalJsonLoader;

        public GlobalJsonManipulator(IGlobalJsonLoader globalJsonLoader)
        {
            _globalJsonLoader = globalJsonLoader;
        }

        public async Task AddMsBuildSdkToGlobalJson(MsBuildSdkInfo sdkInfo)
        {
            var globalJson = await _globalJsonLoader.Load(sdkInfo.Path);

            // TODO: What to do if the version is newer/older
            sdkInfo.SdkList.ForEach(
                msbuildSdk =>
                {
                    if (!globalJson.MsbuildSdk.ContainsKey(msbuildSdk.NuGetPackageId))
                    {
                        globalJson.MsbuildSdk.Add(msbuildSdk.NuGetPackageId, msbuildSdk.Version);
                    }
                });

            await _globalJsonLoader.Store(sdkInfo.Path, globalJson);
        }
    }
}
