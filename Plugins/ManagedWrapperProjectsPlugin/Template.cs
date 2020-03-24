using System;

namespace Messerli.ManagedWrapperProjectsPlugin
{
    internal static class Template
    {
        public static string DllMain => WithPath("dllmain.Source.template");

        public static string PackagesConfig => WithPath("packages.config,template");

        public static string VariableDeclarations => WithPath("VariableDeclarations.json");

        public static string ProjectFile => WithPath("ProjectName.vcxproj.template");

        public static string ProjectFilter => WithPath("ProjectName.vcxproj.filters.template");

        public static string SampleSource => WithPath("Sample.Source.template");

        public static string SampleHeader => WithPath("Sample.Header.template");

        public static string StdAfxSource => WithPath("stdafx.Source.template");

        public static string StdAfxHeader => WithPath("stdafx.Header.template");

        public static string DebugProperties => WithPath("ProjectDebug.props.template");

        public static string ReleaseProperties => WithPath("ProjectRelease.props.template");

        public static string UseProperties => WithPath("UseProject.props.template");

        public static string Resource => WithPath("Resource.template");

        public static string VersionInfo => WithPath("Version.Header.template");

        public static string FilesToSign => WithPath("FilesToSign.template");

        private static string WithPath(string file) => $"Messerli.ManagedWrapperProjectsPlugin.templates.{file}";
    }
}