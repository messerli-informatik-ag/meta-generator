using System;
using System.Collections.Generic;
using Funcky.Monads;

namespace Messerli.VsSolution.Model
{
    public class Project
    {
        public Project(string projectName, string projectPath, ProjectType.Identifier projectType, Option<Guid> projectGuid = default)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            ProjectType = new ProjectType(projectType);
            ProjectGuid = projectGuid.GetOrElse(Guid.NewGuid);
        }

        public Project(string projectName, string projectPath, Guid typeGuid, Guid projectGuid)
        {
            ProjectName = projectName;
            ProjectPath = projectPath;
            ProjectType = new ProjectType(typeGuid);
            ProjectGuid = projectGuid;
        }

        public List<SolutionItem> SolutionItems { get; } = new();

        public string ProjectName { get; }

        public string ProjectPath { get; }

        public Guid ProjectGuid { get; }

        public ProjectType ProjectType { get; }

        public Dictionary<PlatformConfiguration, List<PlatformConfiguration>> Configuration { get; } = new();

        public List<Dependency> Dependencies { get; } = new();
    }
}
