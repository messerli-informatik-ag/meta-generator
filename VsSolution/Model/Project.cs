using System;
using System.Collections.Generic;

namespace Messerli.VsSolution.Model
{
    public class Project
    {
        public Project(string projectName, string projectPath, ProjectType.Identifier projectType)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            ProjectType = new ProjectType(projectType);
            ProjectGuid = Guid.NewGuid();
        }

        public Project(string projectName, string projectPath, Guid typeGuid, Guid projectGuid)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            ProjectType = new ProjectType(typeGuid);
            ProjectGuid = projectGuid;
        }

        public List<SolutionItem> SolutionItems { get; } = new List<SolutionItem>();

        public string ProjectName { get; }

        public string ProjectPath { get; }

        public Guid ProjectGuid { get; }

        public ProjectType ProjectType { get; }

        public Dictionary<PlatformConfiguration, List<PlatformConfiguration>> Configuration { get; } = new Dictionary<PlatformConfiguration, List<PlatformConfiguration>>();

        public List<Dependency> Dependencies { get; } = new List<Dependency>();
    }
}