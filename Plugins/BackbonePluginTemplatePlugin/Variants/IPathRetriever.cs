namespace Messerli.BackbonePluginTemplatePlugin.Variants
{
    internal interface IPathRetriever
    {
        public string GetPath(string fileName);

        public string GetDatabasePath(string fileName);

        public string GetMigrationsPath(string fileName);

        public string GetGuiPath(string fileName);

        public string GetViewPath(string fileName);

        public string GetIconPath(string fileName);

        public string GetRegistrarPath(string fileName);

        public string GetViewTemplatePath(string fileName);

        public string GetTestPath(string pluginName, string fileName);
    }
}