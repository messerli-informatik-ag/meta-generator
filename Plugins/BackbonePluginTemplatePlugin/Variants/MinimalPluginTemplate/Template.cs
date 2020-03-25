namespace Messerli.BackbonePluginTemplatePlugin.Variants.MinimalPluginTemplate
{
    internal static class Template
    {
        public static string ProjectFile => WithPath("Project.csproj.template");

        public static string PluginSource => WithPath("Plugin.Source.template");

        public static string TestProjectFile => WithPath("Project.Test.csproj.template");

        public static string IntegrationTestSource => WithPath("IntegrationTests.Source.template");

        private static string WithPath(string rest) => $"Messerli.BackbonePluginTemplatePlugin.templates.MinimalPluginTemplate.{rest}";
    }
}
