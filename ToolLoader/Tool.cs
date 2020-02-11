using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.ToolLoader
{
    public class Tool : ITool
    {
        private readonly string _path;

        internal Tool(string path)
        {
            Debug.Assert(File.Exists(path), $"No '{nameof(path)}' given");

            _path = path;
        }

        public void Execute(IEnumerable<string> arguments, string workingDirectory)
        {
            Console.WriteLine($"Execute '{_path} {string.Join(" ", arguments)}' in {workingDirectory}");

            var paketInstall = new ProcessStartInfo(_path)
            {
                WorkingDirectory = workingDirectory,
            };

            foreach (var argument in arguments)
            {
                paketInstall.ArgumentList.Add(argument);
            }

            using var process = Process.Start(paketInstall);

            process?.WaitForExit();
        }

        public bool IsAvailable()
        {
            return true;
        }
    }
}