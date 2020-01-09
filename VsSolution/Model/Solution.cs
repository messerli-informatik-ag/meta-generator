using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Messerli.VsSolution.Model
{
    public class Solution
    {
        public int FormatVersion { get; set; }

        public Version VisualStudioVersion { get; set; } = new Version();

        public Version MinimumVisualStudioVersion { get; set; } = new Version();

        public Guid Guid { get; set; }

        public List<Project> Projects { get; } = new List<Project>();

        public List<PlatformConfiguration> Platforms { get; } = new List<PlatformConfiguration>();

        public void AddProject(string projectName, string projectPath, ProjectType projectType)
        {
            Projects.Add(new Project(projectName, projectPath, projectType));
        }
    }
}
