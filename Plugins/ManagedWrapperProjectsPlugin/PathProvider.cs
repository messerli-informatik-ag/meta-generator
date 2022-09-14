using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.ManagedWrapperProjectsPlugin;

internal class PathProvider : IPathProvider
{
    private readonly IUserInputProvider _userInputProvider;
    private readonly ITools _tools;
    private static readonly HashSet<string> NoBranch = new() { "RDDs", "BuildProcessTemplates", "Settings", "TeamProjectConfig", "VersInfo" };

    public PathProvider(IUserInputProvider userInputProvider, ITools tools)
    {
        _userInputProvider = userInputProvider;
        _tools = tools;
    }

    public IEnumerable<SelectionValue> GetBranches()
    {
        return Directory
            .GetDirectories(GetRmiProdRoot())
            .Where(path => IsBranch(Path.GetFileName(path)))
            .Select(path => new SelectionValue { Value = path, Description = Path.GetFileName(path) });
    }

    public string GetVersionInfoPath()
    {
        return Path.Combine(GetRmiProdRoot(), "VersInfo", _userInputProvider.Value(Variable.Branch));
    }

    public string GetSolutionDirectory()
    {
        return Path.Combine(GetBranchPath(), "AllProjects");
    }

    public string GetBuildStepSignDirectory()
    {
        return Path.Combine(GetBranchPath(), "Build", "AdditionalBuildSteps", "Sign");
    }

    public string GetVisualStudioToolDirectory()
    {
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var vswhere = _tools.CreateToolFromPath($"{programFiles}\\Microsoft Visual Studio\\Installer\\vswhere.exe");

        vswhere.Execute(new[] { "-latest" }, ".");

        var basePath = ParseVsWhere(vswhere.StandardOutput, "installationPath");

        return Path.Combine(basePath, @"Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer");
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

    private string ParseVsWhere(string vswhereStandardOutput, string filter)
    {
        string[] lines = vswhereStandardOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        return lines
            .First(line => line.StartsWith(filter))
            .Substring(filter.Length + 2)
            .Trim();
    }

    private bool IsBranch(string possibleBranch)
    {
        return NoBranch.Contains(possibleBranch) == false;
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
