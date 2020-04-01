using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions;

namespace Messerli.FileManipulator
{
    public sealed class GlobalJsonManipulator : IGlobalJsonManipulator
    {
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

        private Task CreateGlobalJson(MsBuildSdkInfo sdk)
        {
            throw new NotImplementedException();
        }
    }
}
