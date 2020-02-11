using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Messerli.VsSolution.Model
{
    public class Solution
    {
        private const string ActiveConfiguration = "ActiveCfg";
        private const string BuildZero = "Build.0";

        public Solution(string solutionPath)
        {
            SolutionPath = solutionPath;
        }

        public string SolutionPath { get; }

        public int FormatVersion { get; set; }

        public Version VisualStudioVersion { get; set; } = new Version();

        public Version MinimumVisualStudioVersion { get; set; } = new Version();

        public Guid Guid { get; set; }

        public List<Project> Projects { get; } = new List<Project>();

        public List<PlatformConfiguration> Platforms { get; } = new List<PlatformConfiguration>();

        public List<SolutionProperty> Properties { get; } = new List<SolutionProperty>();

        public List<NestedProject> ProjectNesting { get; } = new List<NestedProject>();

        public List<TfsControlProperty> TfsControlProperties { get; } = new List<TfsControlProperty>();

        public void AddProject(string projectName, string projectPath, ProjectType.Identifier projectType, Guid? projectGuid)
        {
            var project = new Project(projectName, PathRelativeToSolution(projectPath), projectType, projectGuid);

            foreach (var platform in Platforms)
            {
                project.Configuration[platform] = new List<PlatformConfiguration>
                {
                    new PlatformConfiguration(ActiveConfiguration, platform.Config),
                    new PlatformConfiguration(BuildZero, platform.Config),
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

            return Path.GetRelativePath(solutionDirectory, projectPath);
        }
    }
}
