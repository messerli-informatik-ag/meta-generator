using System.Collections.Generic;
using System.IO;
using System.Linq;
using Messerli.MetaGeneratorAbstractions;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.MetaGenerator
{
    public class DotnetToolInstaller : IDotnetToolInstaller
    {
        private const string Dotnet = "dotnet";
        private const string DotnetExe = "dotnet.exe";

        private readonly ITools _tools;

        public DotnetToolInstaller(ITools tools)
        {
            _tools = tools;
        }

        public void RegisterTool()
            => _tools.RegisterTool(Dotnet, DotnetExe);

        public void InstallTool(string path, string toolName)
        {
            AddToolManifestIfNotExists(path);
            Execute(GetToolInstallArguments(toolName), path);
        }

        public void InstallTool(string path, string toolName, string version)
        {
            AddToolManifestIfNotExists(path);
            Execute(GetToolInstallArguments(toolName, version), path);
        }

        private static bool HasDotNetToolManifest(string path)
        {
            const string dotnetToolsFileName = "dotnet-tools.json";
            return Directory.GetFiles(path, dotnetToolsFileName, SearchOption.AllDirectories)
                .Any();
        }

        private void AddToolManifestIfNotExists(string path)
        {
            var hasDotnetToolManifest = HasDotNetToolManifest(path);
            if (!hasDotnetToolManifest)
            {
                Execute(GetNewToolManifestArguments(), path);
            }
        }

        private void Execute(IEnumerable<string> arguments, string path)
        {
            var dotnet = _tools.GetTool(Dotnet);
            dotnet.Execute(arguments, path);
        }

        private static IEnumerable<string> GetToolInstallArguments(string toolName, string version)
        {
            var arguments = GetToolInstallArguments(toolName);
            return arguments.Concat(GetVersionArguments(version));
        }

        private static IEnumerable<string> GetNewToolManifestArguments()
            => new[]
            {
                "new",
                "tool-manifest",
            };

        private static IEnumerable<string> GetToolInstallArguments(string toolName)
            => new[]
            {
                "tool",
                "install",
                toolName,
            };

        private static IEnumerable<string> GetVersionArguments(string version)
            => new[]
            {
                "--version",
                version,
            };
    }
}
