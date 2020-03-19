using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.ManagedWrapperProjectsPlugin
{
    internal class PathProvider : IPathProvider
    {
        private readonly IUserInputProvider _userInputProvider;
        private static readonly HashSet<string> _noBranch = new HashSet<string> { "RDDs", "BuildProcessTemplates", "Settings", "TeamProjectConfig", "VersInfo" };

        public PathProvider(IUserInputProvider userInputProvider)
        {
            _userInputProvider = userInputProvider;
        }

        public IEnumerable<SelectionValue> GetBranches()
        {
            return Directory
                .GetDirectories(GetRmiProdRoot())
                .Where(path => IsBranch(Path.GetFileName(path)))
                .Select(path => new SelectionValue { Value = path, Description = Path.GetFileName(path) });
        }

        public string GetSolutionPath()
        {
            return Path.Combine(GetBranchPath(), "All");
        }

        public string GetProjectPath()
        {
            return Path.Combine(GetBranchPath(), _userInputProvider.Value(Variable.ProjectPath), _userInputProvider.Value(Variable.ProjectName));
        }

        public string GetTfsProjectRoot()
        {
            return Environment.GetEnvironmentVariable(EnvironmentVariable.TfsProjectRoot)
                                ?? throw new ArgumentNullException($"Environment variable '{EnvironmentVariable.TfsProjectRoot}' not set");
        }

        private bool IsBranch(string possibleBranch)
        {
            return _noBranch.Contains(possibleBranch) == false;
        }

        private string GetBranchPath()
        {
            return Path.Combine(GetRmiProdRoot(), _userInputProvider.Value(Variable.Branch));
        }

        private string GetRmiProdRoot()
        {
            return Path.Combine(GetTfsProjectRoot(), "RMIProd");
        }
    }
}
