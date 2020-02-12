using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.ToolLoader
{
    public class Tools : ITools
    {
        private readonly List<Tuple<string, Func<ITool>>> _findTools = new List<Tuple<string, Func<ITool>>>();
        private Dictionary<string, ITool> _tools = new Dictionary<string, ITool>();
        private Tool.Factory _toolFactory;

        public Tools(Tool.Factory toolFactory)
        {
            _toolFactory = toolFactory;
        }

        public void RegisterTool(string name, string executable, string? specificPath = null)
            => _findTools.Add(Tuple.Create(name, (Func<ITool>)(() => FindTool(executable, specificPath))));

        public IEnumerable<string> VerifyTools()
        {
            _tools = _findTools.ToDictionary(ToKey, ToValue);

            return _tools
                .Where(tool => tool.Value.IsAvailable() == false)
                .Select(tool => tool.Key);
        }

        public ITool GetTool(string name) => _tools[name];

        private string ToKey(Tuple<string, Func<ITool>> tuple) => tuple.Item1;

        private ITool ToValue(Tuple<string, Func<ITool>> tuple) => tuple.Item2.Invoke();

        private ITool FindTool(string executable, string? specificPath) =>
                    FindToolPath(executable, specificPath) is { } toolPath
                        ? _toolFactory(toolPath)
                        : NullTool.Create();

        private string? FindToolPath(string executable, string? specificPath) =>
            specificPath is null
                ? GetFullPathInPathVariable(executable)
                : GetFullPathFromSpecificPath(executable, specificPath);

        private string? GetFullPathFromSpecificPath(string executable, string specificPath) =>
            File.Exists(Path.Combine(specificPath, executable))
                ? Path.Combine(specificPath, executable)
                : null;

        private static string? GetFullPathInPathVariable(string executable) =>
            File.Exists(executable)
                ? Path.GetFullPath(executable)
                : Environment.GetEnvironmentVariable("PATH")
                    ?.Split(Path.PathSeparator)
                    .Select(path => Path.Combine(path, executable))
                    .FirstOrDefault(File.Exists);
    }
}
