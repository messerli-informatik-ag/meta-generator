using static Messerli.BackbonePluginTemplatePlugin.Variants.TemplateConstant;

namespace Messerli.BackbonePluginTemplatePlugin.Variants
{
    internal sealed class PathRetriever : IPathRetriever
    {
        private readonly string _root;
        private readonly string _separator;

        public PathRetriever(string root, string separator)
        {
            _root = root;
            _separator = separator;
        }

        public string GetPath(string fileName) => _root + _separator + fileName;

        public string GetDatabasePath(string fileName) => GetPath(DatabaseFolder + _separator + fileName);

        public string GetMigrationsPath(string fileName) => GetPath(MigrationFolder + _separator + fileName);

        public string GetGuiPath(string fileName) => GetPath(GuiFolder + _separator + fileName);

        public string GetViewPath(string fileName) => GetPath(GuiFolder + _separator + ViewFolder + _separator + fileName);

        public string GetIconPath(string fileName) => GetPath(PublicFolder + _separator + IconsFolder + _separator + fileName);

        public string GetRegistrarPath(string fileName) => GetPath(RegistrarFolder + _separator + fileName);

        public string GetViewTemplatePath(string fileName) => GetPath(TemplateFolder + _separator + fileName);

        public string GetTestPath(string pluginName, string fileName) => GetPath(pluginName + ".Test" + _separator + fileName);
    }
}