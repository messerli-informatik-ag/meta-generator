namespace Messerli.BackbonePluginTemplatePlugin.Variants.MinimalPlugin
{
    internal static class Template
    {
        public static string ProjectFile => GetPath("Project.csproj.template");

        public static string PluginSource => GetPath("Plugin.Source.template");

        public static string TestProjectFile => GetPath("Project.Test.csproj.template");

        public static string IntegrationTestSource => GetPath("IntegrationTests.Source.template");

        private static string GetPath(string rest) => $"Messerli.BackbonePluginTemplatePlugin.templates.MinimalPluginTemplate.{rest}";
    }
}
