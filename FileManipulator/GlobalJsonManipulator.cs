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
        private readonly IFileOpeningBuilder _fileOpeningBuilder;

        public GlobalJsonManipulator(IFileOpeningBuilder fileOpeningBuilder)
        {
            _fileOpeningBuilder = fileOpeningBuilder;
        }

        public async Task ModifyGlobalJson(string path, GlobalJsonModification modification)
        {
            var document = await GetJsonDocument(path);
            AddMsBuildSdks(document, modification.MsBuildSdksToAdd);
            await WriteJsonFile(path, document);
        }

        private static void AddMsBuildSdks(JObject document, IEnumerable<MsBuildSdk> sdks)
        {
            var sdkList = document.GetOrInsert<JObject>("msbuild-sdks");

            foreach (var sdk in sdks)
            {
                AddMsBuildSdk(sdkList, sdk);
            }
        }

        private static void AddMsBuildSdk(JObject sdkList, MsBuildSdk sdk)
        {
            var existingSdkVersion = sdkList.GetValue(sdk.NuGetPackageId) is { } value ? (string?)value : null;

            if (existingSdkVersion is { } version && sdk.Version != version)
            {
                throw new InvalidOperationException(
                    $"MSBuild SDK '{sdk.NuGetPackageId}' already exists with version '{existingSdkVersion}'");
            }

            sdkList[sdk.NuGetPackageId] = sdk.Version;
        }

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
