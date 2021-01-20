using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.ToolLoader
{
    public class Tool : ITool
    {
        private readonly IConsoleWriter _consoleWriter;
        private readonly string _path;
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public Tool(IConsoleWriter consoleWriter, string path)
        {
            Debug.Assert(File.Exists(path), $"No '{nameof(path)}' given");
            _consoleWriter = consoleWriter;
            _path = path;
        }

        public delegate Tool Factory(string path);

        public string StandardOutput => _stringBuilder.ToString();

        public void Execute(IEnumerable<string> arguments, string workingDirectory)
        {
            _stringBuilder.Clear();
            _consoleWriter.WriteLine($"Execute '{_path} {string.Join(" ", arguments)}' in {workingDirectory}");

            using var process = Process.Start(CreateProcessStartInfo(arguments, workingDirectory));

            Option
                .FromNullable(process)
                .AndThen(p => RecursiveRead(p, _stringBuilder));
        }

        public bool IsAvailable()
        {
            return true;
        }

        private void RecursiveRead(Process process, StringBuilder stringBuilder)
        {
            if (process.HasExited)
            {
                return;
            }

            _stringBuilder.Append(process.StandardOutput.ReadToEnd());
            RecursiveRead(process, stringBuilder);
        }

        private ProcessStartInfo CreateProcessStartInfo(IEnumerable<string> arguments, string workingDirectory)
        {
            var startInfo = new ProcessStartInfo(_path)
            {
                WorkingDirectory = workingDirectory,
            };

            foreach (var argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }

            startInfo.RedirectStandardOutput = true;
            return startInfo;
        }
    }
}
