using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions;
using Messerli.FileSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Messerli.FileManipulator
{
    public sealed class GlobalJsonManipulator : IGlobalJsonManipulator
    {
        private const string MsBuildSdksJsonProperty = "msbuild-sdks";

        private readonly IFileOpeningBuilder _fileOpeningBuilder;

        public GlobalJsonManipulator(IFileOpeningBuilder fileOpeningBuilder)
        {
            _fileOpeningBuilder = fileOpeningBuilder;
        }

        public async Task ModifyGlobalJson(string filePath, GlobalJsonModification modification)
            => await WrapExceptions(filePath, async () =>
            {
                var document = await GetJsonDocument(filePath);
                AddMsBuildSdks(document, modification.MsBuildSdksToAdd);
                await WriteJsonFile(filePath, document);
            });

        private static async Task WrapExceptions(string filePath, Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception exception)
            {
                throw new GlobalJsonManipulationException(exception, filePath);
            }
        }

        private static void AddMsBuildSdks(JObject document, IEnumerable<MsBuildSdk> sdks)
        {
            var sdkList = document.GetOrInsert(MsBuildSdksJsonProperty, () => new JObject());
            var sdkListObject = sdkList as JObject
                ?? throw new MalformedGlobalJsonException(
                    $"Key '{MsBuildSdksJsonProperty}' is expected to be an object, but is of type {sdkList.Type}");

            foreach (var sdk in sdks)
            {
                AddMsBuildSdk(sdkListObject, sdk);
            }
        }

        private static void AddMsBuildSdk(JObject sdkList, MsBuildSdk sdk)
        {
            var existingSdkVersion = sdkList.GetValue(sdk.NuGetPackageId) is { } value ? (string?)value : null;

            if (existingSdkVersion is { } version && !VersionAreCompatible(version, sdk.Version))
            {
                throw new ConflictingMsBuildSdkException(sdk, existingSdkVersion);
            }

            sdkList[sdk.NuGetPackageId] = sdk.Version;
        }

        private static bool VersionAreCompatible(string currentVersion, string newVersion)
            => currentVersion == newVersion;

        private async Task<JObject> GetJsonDocument(string path)
        {
            try
            {
                return await ParseExistingJsonFile(path);
            }
            catch (FileNotFoundException)
            {
                return new JObject();
            }
        }

        private async Task<JObject> ParseExistingJsonFile(string path)
        {
            await using var stream = _fileOpeningBuilder
                .Read(true)
                .Open(path);
            using var streamReader = new StreamReader(stream);
            using var textReader = new JsonTextReader(streamReader);
            return (JObject)await JToken.ReadFromAsync(textReader);
        }

        private async Task WriteJsonFile(string path, JToken document)
        {
            await using var stream = _fileOpeningBuilder
                .Write(true)
                .Create(true)
                .Truncate(true)
                .Open(path);
            await using var streamWriter = new StreamWriter(stream);
            using var jsonTextWriter = new JsonTextWriter(streamWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
            };
            await document.WriteToAsync(jsonTextWriter);
        }
    }
}
