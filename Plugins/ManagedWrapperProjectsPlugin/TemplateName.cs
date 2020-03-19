using System;

namespace Messerli.ManagedWrapperProjectsPlugin
{
    internal static class TemplateName
    {
        public static string DllMain => Template("dllmain.Source.template");

        public static string PackagesConfig => Template("packages.config,template");

        public static string VariableDeclarations => Template("VariableDeclarations.json");

        public static string ProjectFile => Template("ProjectName.vcxproj.template");

        public static string ProjectFilter => Template("ProjectName.vcxproj.filters.template");

        public static string SampleSource => Template("Sample.Source.template");

        public static string SampleHeader => Template("Sample.Header.template");

        public static string StdAfxSource => Template("stdafx.Header.template");

        public static string StdAfxHeader => Template("stdafx.Header.template");

        public static string DebugProperties => Template("ProjectDebug.props.template");

        public static string ReleaseProperties => Template("ProjectRelease.props.template");

        public static string UseProperties => Template("UseProject.props.template");

        public static string Resource => Template("Resource.template");

        private static string Template(string rest) => $"Messerli.ManagedWrapperProjectsPlugin.templates.{rest}";
    }
}