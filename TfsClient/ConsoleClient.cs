using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Messerli.CommandLineAbstractions;

namespace Messerli.TfsClient
{
    public class ConsoleClient : IClient
    {
        private readonly IConsoleWriter _consoleWriter;
        private readonly string _tfPath;

        public ConsoleClient(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
            _tfPath = CreateTfPath(GetVsPath());
        }

        public void AddToWorkspace(string path)
        {
            _consoleWriter.WriteLine($"TFS: add '{path}' to work space");
            StartTfProcess("add " + path + " /recursive");
        }

        public void CheckOutFile(string path)
        {
            _consoleWriter.WriteLine($"TFS: check out '{path}'");
            StartTfProcess("checkout " + path);
        }

        public string GetPath()
        {
            var workfold = GetProcessData(_tfPath, "vc workfold");
            var path = GetNeedle(workfold, ": (.+\\\\.+)");
            return IncludeTfsRoot(path);
        }

        private void StartTfProcess(string arguments)
        {
            using (var process = new Process
            {
                StartInfo =
                {
                    FileName = _tfPath,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                },
            })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        private static string IncludeTfsRoot(string path)
        {
            const string tfsRoot = "RMIProd";

            return path.IndexOf(tfsRoot, StringComparison.Ordinal) == -1
                ? Path.Combine(path, tfsRoot)
                : path;
        }

        private static string CreateTfPath(string visualStudioPath)
        {
            return Path.Combine(visualStudioPath, "Common7", "IDE", "CommonExtensions", "Microsoft", "TeamFoundation", "Team Explorer", "TF.exe");
        }

        private static string GetVsPath()
        {
            var processData =
                GetProcessData(Environment.ExpandEnvironmentVariables(Path.Combine("%ProgramFiles(x86)%", "Microsoft Visual Studio", "Installer", "vswhere.exe")), "-latest");

            processData = GetNeedle(processData, "installationPath: (.*)");
            return processData;
        }

        private static string GetProcessData(string path, string arguments)
        {
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                },
            })
            {
                process.Start();
                var line = string.Empty;
                while (!process.StandardOutput.EndOfStream)
                {
                    line += process.StandardOutput.ReadLine();
                    line += "\n";
                }

                return line;
            }
        }

        private static string GetNeedle(string processData, string regexNeedle)
        {
            var value = Regex.Match(processData, regexNeedle).Groups[1];
            return value.ToString();
        }
    }
}