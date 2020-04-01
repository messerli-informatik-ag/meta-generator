using System;
using Newtonsoft.Json.Linq;

namespace Messerli.FileManipulator
{
    internal static class JObjectExtension
    {
        public static JToken GetOrInsert(this JObject @object, string propertyName, Func<JToken> getDefaultValue)
            => @object.TryGetValue(propertyName, out var value)
                ? value
                : @object[propertyName] = getDefaultValue();
    }
}