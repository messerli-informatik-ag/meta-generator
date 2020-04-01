using Newtonsoft.Json.Linq;

namespace Messerli.FileManipulator
{
    internal static class JObjectExtension
    {
        public static T GetOrInsert<T>(this JObject @object, string propertyName)
            where T : JToken, new()
            => @object.TryGetValue(propertyName, out var value)
                ? (T)value
                : InsertProperty<T>(@object, propertyName);

        private static T InsertProperty<T>(JObject @object, string propertyName)
            where T : JToken, new()
        {
            var propertyValue = new T();
            @object[propertyName] = propertyValue;
            return propertyValue;
        }
    }
}