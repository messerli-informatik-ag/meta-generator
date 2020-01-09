using System;
using System.Collections.Generic;

namespace Messerli.VsSolution.Model
{
    public class Project
    {
        public Project(string projectName, string projectPath, ProjectType projectType)
        {
            ProjectGuid = Guid.NewGuid();
            TypeGuid = GuidForProjectType(projectType);  // SDK-style projects
            ProjectName = projectName;
            ProjectPath = projectPath;
        }

        public Project(Guid projectGuid, Guid typeGuid, string projectName, string projectPath)
        {
            ProjectGuid = projectGuid;
            TypeGuid = typeGuid;
            ProjectName = projectName;
            ProjectPath = projectPath;
        }

        public List<SolutionItem> SolutionItems { get; } = new List<SolutionItem>();

        public string ProjectName { get; }

        public string ProjectPath { get; }

        public Guid ProjectGuid { get; }

        public Guid TypeGuid { get; }

        public Dictionary<PlatformConfiguration, List<PlatformConfiguration>> Configuration { get; } = new Dictionary<PlatformConfiguration, List<PlatformConfiguration>>();

        private Guid GuidForProjectType(ProjectType projectType)
        {
            return projectType switch
            {
                ProjectType.DotNetStandard => Guid.Parse("9A19103F-16F7-4668-BE54-9A1E7A4F7556"),
                _ => throw new NotImplementedException("Unknown VS Project Type")
            };
        }
    }
}