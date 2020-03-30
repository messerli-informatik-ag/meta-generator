using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Messerli.FileManipulator
{
    public class GlobalJsonLoader : IGlobalJsonLoader
    {
        public async Task<GlobalJson> Load(string path)
        {
            try
            {
                var jsonString = await File.ReadAllTextAsync(path);
                return JsonSerializer.Deserialize<GlobalJson>(jsonString);
            }
            catch (FileNotFoundException)
            {
                return new GlobalJson();
            }
        }

        public async Task Store(string path, GlobalJson globalJson)
        {
            var writeIndented = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(globalJson, writeIndented));
        }
    }
}
