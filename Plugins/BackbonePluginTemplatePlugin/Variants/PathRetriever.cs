using static Messerli.BackbonePluginTemplatePlugin.Variants.TemplateConstant;

namespace Messerli.BackbonePluginTemplatePlugin.Variants
{
    internal sealed class PathRetriever : IPathRetriever
    {
        private readonly string _root;
        private readonly string _pluginName;
        private readonly string _separator;

        public PathRetriever(string root, string pluginName, string separator)
        {
            _root = root;
            _pluginName = pluginName;
            _separator = separator;
        }

        public string GetPluginPath(string fileName) => _root + _separator + _pluginName + _separator + fileName;

        public string GetDatabasePath(string fileName) => GetPluginPath(DatabaseFolder + _separator + fileName);

        public string GetMigrationsPath(string fileName) => GetPluginPath(MigrationFolder + _separator + fileName);

        public string GetGuiPath(string fileName) => GetPluginPath(GuiFolder + _separator + fileName);

        public string GetViewPath(string fileName) => GetPluginPath(GuiFolder + _separator + ViewFolder + _separator + fileName);

        public string GetIconPath(string fileName) => GetPluginPath(PublicFolder + _separator + IconsFolder + _separator + fileName);

        public string GetRegistrarPath(string fileName) => GetPluginPath(RegistrarFolder + _separator + fileName);

        public string GetViewTemplatePath(string fileName) => GetPluginPath(TemplateFolder + _separator + fileName);

        public string GetTestPath(string fileName) => _root + _separator + _pluginName + ".Test" + _separator + fileName;
    }
}