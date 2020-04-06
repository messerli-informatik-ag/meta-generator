namespace Messerli.BackbonePluginTemplatePlugin
{
    internal static class Template
    {
        public static string VariableDeclarations => WithPath("VariableDeclarations.json");

        private static string WithPath(string rest) => $"Messerli.BackbonePluginTemplatePlugin.templates.{rest}";
    }
}
