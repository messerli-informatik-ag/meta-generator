using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Messerli.FileManipulator
{
    public class GlobalJson
    {
        [JsonPropertyName("msbuild-sdks")]
        public Dictionary<string, string> MsbuildSdk { get; set; } = new Dictionary<string, string>();
    }
}
