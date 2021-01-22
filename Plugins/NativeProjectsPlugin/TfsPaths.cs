using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Messerli.MetaGeneratorAbstractions.Json;

namespace Messerli.NativeProjectsPlugin
{
    internal class TfsPaths : ITfsPaths
    {
        private const string TfsRootVariable = "TFSProjRoot";

        private const string TfsBase = "RmiProd";

        private readonly List<string> _nonBranches = new() { "BuildProcessTemplates", "RDDs", "Settings", "TeamProjectConfig", "VersInfo" };

        public IEnumerable<SelectionValue> Branches()
        {
            return Directory
                .GetDirectories(BranchRootPath())
                .Where(branch => _nonBranches.Contains(Path.GetFileName(branch)) == false)
                .Select(branch => new SelectionValue { Description = Path.GetFileName(branch), Value = branch });
        }

        public string BranchRootPath() => Path.Combine(TfsRootPath(), TfsBase);

        private string TfsRootPath() => Environment.GetEnvironmentVariable(TfsRootVariable)
                                        ?? throw new Exception($"Missing environment variable '{TfsRootVariable}'");
    }
}
