using static Messerli.BackbonePluginTemplatePlugin.Variants.TemplateConstant;

namespace Messerli.BackbonePluginTemplatePlugin.Variants
{
    internal sealed class TemplatePathRetriever : ITemplatePathRetriever
    {
        private readonly string _pluginName;

        public TemplatePathRetriever(string pluginName)
        {
            _pluginName = pluginName;
        }

        public string GetPath(string fileName) => $"Messerli.BackbonePluginTemplatePlugin.templates.{_pluginName}.{fileName}";

        public string GetDatabasePath(string fileName) => GetPath($"{DatabaseFolder}.{fileName}");

        public string GetMigrationsPath(string fileName) => GetPath($"{MigrationFolder}.{fileName}");

        public string GetGuiPath(string fileName) => GetPath($"{GuiFolder}.{fileName}");

        public string GetViewPath(string fileName) => GetPath($"{GuiFolder}.{ViewFolder}.{fileName}");

        public string GetIconPath(string fileName) => GetPath($"{PublicFolder}.{IconsFolder}.{fileName}");

        public string GetRegistrarPath(string fileName) => GetPath($"{RegistrarFolder}.{fileName}");

        public string GetViewTemplatePath(string fileName) => GetPath($"{TemplateFolder}.{fileName}");

        public string GetTestPath(string fileName) => GetPath($"{TestFolder}.{fileName}");
    }
}