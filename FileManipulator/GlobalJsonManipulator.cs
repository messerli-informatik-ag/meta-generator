using System.IO;
using System.Linq;
using System.Text.Json;
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

        public async Task AddMsBuildSdkToGlobalJson(MsBuildSdkInfo sdk)
        {
            try
            {
                var jsonString = await File.ReadAllTextAsync(sdk.Path);
                using var jsonDocument = JsonDocument.Parse(jsonString);
                var msbuildSdksProperty = jsonDocument.RootElement.GetProperty("msbuild-sdks");

                sdk.SdkList.ForEach(
                    msbuildSdk =>
                    {
                        if (!msbuildSdksProperty.TryGetProperty(msbuildSdk.NuGetPackageId, out var sdks))
                        {
                            using FileStream stream = File.Open(sdk.Path, FileMode.CreateNew);
                            using var writer = new Utf8JsonWriter(stream);
                            writer.WriteStartObject();

                            // TODO Write property
                            writer.WriteEndObject();
                        }
                    });
            }
            catch (FileNotFoundException)
            {
                await CreateGlobalJson(sdk);
            }
        }

        private async Task CreateGlobalJson(MsBuildSdkInfo sdk)
        {
            var globalJson = await _globalJsonLoader.Load(sdk.Path);
            sdk.SdkList.ForEach(
                msbuildSdk =>
                {
                    if (!globalJson.MsbuildSdk.ContainsKey(msbuildSdk.NuGetPackageId))
                    {
                        globalJson.MsbuildSdk.Add(msbuildSdk.NuGetPackageId, msbuildSdk.Version);
                    }

                    // TODO What to do if the version is newer/older? --> Warning
                });

            await _globalJsonLoader.Store(sdk.Path, globalJson);
        }
    }
}
