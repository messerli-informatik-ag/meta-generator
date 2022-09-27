using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Funcky.Monads;

namespace Messerli.VsSolution.Model;

public class Solution
{
    private const int CurrentFormatVersion = 12;
    private const string ActiveConfiguration = "ActiveCfg";
    private const string BuildZero = "Build.0";
    private const string DefaultPlatform = "Any CPU";

    public Solution(string solutionPath)
    {
        SolutionPath = solutionPath;
    }

    public string SolutionPath { get; }

    public int FormatVersion { get; set; }

    public Version VisualStudioVersion { get; set; } = new();

    public Version MinimumVisualStudioVersion { get; set; } = new();

    public Guid Guid { get; set; }

    public List<Project> Projects { get; } = new();

    public List<PlatformConfiguration> Platforms { get; } = new();

    public List<SolutionProperty> Properties { get; } = new();

    public List<NestedProject> ProjectNesting { get; } = new();

    public List<TfsControlProperty> TfsControlProperties { get; } = new();

    public static Solution NewSolution(string solutionPath)
    {
        var result = new Solution(solutionPath)
        {
            FormatVersion = CurrentFormatVersion,
            VisualStudioVersion = VisualStudio2019(),
            MinimumVisualStudioVersion = CurrentMinimumVisualStudioVersion(),
            Guid = Guid.NewGuid(),
        };

        result.Platforms.Add(Configuration("Debug"));
        result.Platforms.Add(Configuration("Release"));

        return result;
    }

    public void AddProject(string projectName, string projectPath, ProjectType.Identifier projectType, Option<Guid> projectGuid = default)
    {
        var project = new Project(projectName, PathRelativeToSolution(projectPath), projectType, projectGuid);

        foreach (var platform in Platforms)
        {
            project.Configuration[platform] = new List<PlatformConfiguration>
            {
                new(ActiveConfiguration, platform.Config),
                new(BuildZero, platform.Config),
            };
        }

        Projects.Add(project);
    }

    public void AddNestedProject(string folderName, string projectName)
    {
        var folder = Projects.First(p => p.ProjectName == folderName);
        var project = Projects.First(p => p.ProjectName == projectName);

        ProjectNesting.Add(new NestedProject(folder.ProjectGuid, project.ProjectGuid));
    }

    private string PathRelativeToSolution(string projectPath)
    {
        var solutionDirectory = Path.GetDirectoryName(SolutionPath);

        return string.IsNullOrEmpty(solutionDirectory)
            ? throw new Exception("solutionDirectory is null or empty")
            : Path.GetRelativePath(solutionDirectory, projectPath);
    }

    private static PlatformConfiguration Configuration(string configuration)
        => new(ConfigurationPlatform(configuration, DefaultPlatform), ConfigurationPlatform(configuration, DefaultPlatform));

    private static string ConfigurationPlatform(string configuration, string platform)
        => $"{configuration}|{platform}";

    private static Version CurrentMinimumVisualStudioVersion()
        => new(10, 0, 40219, 1);

    private static Version VisualStudio2019()
        => new(16, 0, 29709, 97);
}
