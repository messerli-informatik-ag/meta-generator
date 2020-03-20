using System;
using System.Collections.Generic;
using System.Text;

namespace Messerli.MesserliOneRepositoryPlugin
{
    internal static class Template
    {
        public static string VariableDeclarations => WithPath("VariableDeclarations.json");

        public static string DirectoryBuildTargets => WithPath("Directory.Build.targets.template");

        public static string GitIgnore => WithPath("gitignore.template");

        public static string ProjectFile => WithPath("Project.csproj.template");

        public static string PublishScript => WithPath("publish.ps1.template");

        public static string GlobalJson => WithPath("global.json.template");

        public static string ProgramSource => WithPath("Program.Source.template");

        public static string ApplicationSource => WithPath("Application.Source.template");

        public static string ApplicationInterfaceSource => WithPath("IApplication.Source.template");

        public static string CompositionRootSource => WithPath("CompositionRoot.Source.template");

        public static string PackagesProperties => WithPath("Packages.props.template");

        private static string WithPath(string rest) => $"Messerli.MesserliOneRepositoryPlugin.templates.{rest}";
    }
}
