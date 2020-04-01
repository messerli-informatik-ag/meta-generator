using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions;

namespace Messerli.FileManipulator
{
    public sealed class GlobalJsonManipulator : IGlobalJsonManipulator
    {
        public async Task AddMsBuildSdkToGlobalJson(string path, GlobalJsonModification modification)
        {
            try
            {
                var jsonString = await File.ReadAllTextAsync(path);
                using var jsonDocument = JsonDocument.Parse(jsonString);
                var msbuildSdksProperty = jsonDocument.RootElement.GetProperty("msbuild-sdks");

                modification.SdksToAdd.ForEach(
                    msbuildSdk =>
                    {
                        if (!msbuildSdksProperty.TryGetProperty(msbuildSdk.NuGetPackageId, out var sdks))
                        {
                            using FileStream stream = File.Open(modification.Path, FileMode.CreateNew);
                            using var writer = new Utf8JsonWriter(stream);
                            writer.WriteStartObject();

                            // TODO Write property
                            writer.WriteEndObject();
                        }
                    });
            }
            catch (FileNotFoundException)
            {
                await CreateGlobalJson(modification);
            }
        }

        private Task CreateGlobalJson(GlobalJsonModification sdk)
        {
            throw new NotImplementedException();
        }
    }
}
