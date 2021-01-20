using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.ToolLoader
{
    public class Tools : ITools
    {
        private readonly Tool.Factory _toolFactory;
        private readonly IExecutingPluginAssemblyProvider _executingPluginAssembly;
        private readonly List<Tuple<string, Func<ITool>>> _findTools = new ();
        private Dictionary<string, ITool> _tools = new ();

        public Tools(Tool.Factory toolFactory, IExecutingPluginAssemblyProvider executingPluginAssembly)
        {
            _toolFactory = toolFactory;
            _executingPluginAssembly = executingPluginAssembly;
        }

        public void RegisterTool(string name, string executable, Option<string> specificPath = default)
            => _findTools.Add(Tuple.Create(UniqueName(name), (Func<ITool>)(() => FindTool(executable, specificPath))));

        public IEnumerable<KeyValuePair<string, ITool>> VerifyTools()
        {
            _tools = _findTools.ToDictionary(ToKey, ToValue);

            return _tools
                .Where(tool => tool.Value.IsAvailable() == false);
        }

        public ITool GetTool(string name) => _tools[UniqueName(name)];

        public ITool CreateToolFromPath(string path) => _toolFactory(path);

        private string UniqueName(string name) => $"{PluginContext()}::{name}";

        private string PluginContext()
            => _executingPluginAssembly.HasPluginContext
                ? _executingPluginAssembly.PluginAssembly.GetName().Name ?? throw new Exception("Missing assembly name?")
                : "GLOBAL";

        private string ToKey(Tuple<string, Func<ITool>> tuple) => tuple.Item1;

        private ITool ToValue(Tuple<string, Func<ITool>> tuple) => tuple.Item2.Invoke();

        private ITool FindTool(string executable, Option<string> specificPath) =>
                    FindToolPath(executable, specificPath)
                    .Match(
                        none: () => NullTool.Create(),
                        some: path => _toolFactory(path));

        private Option<string> FindToolPath(string executable, Option<string> specificPath) =>
            specificPath.Match(
                none: () => GetFullPathInPathVariable(executable),
                some: path => GetFullPathFromSpecificPath(executable, path));

        private Option<string> GetFullPathFromSpecificPath(string executable, string specificPath) =>
            File.Exists(Path.Combine(specificPath, executable))
                ? Path.Combine(specificPath, executable)
                : Option<string>.None();

        private static Option<string> GetFullPathInPathVariable(string executable) =>
            File.Exists(executable)
                ? Path.GetFullPath(executable)
                : Option
                    .FromNullable(Environment.GetEnvironmentVariable("PATH"))
                    .AndThen(p => p.Split(Path.PathSeparator))
                    .AndThen(p => p.Select(path => Path.Combine(path, executable)).FirstOrNone());
    }
}
