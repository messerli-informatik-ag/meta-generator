namespace Messerli.BackbonePluginTemplatePlugin.Variants
{
    internal interface IPathPairRetriever
    {
        public PathPair GetPluginPath(string fileName);

        public PathPair GetDatabasePath(string fileName);

        public PathPair GetMigrationsPath(string fileName);

        public PathPair GetGuiPath(string fileName);

        public PathPair GetViewPath(string fileName);

        public PathPair GetIconPath(string fileName);

        public PathPair GetRegistrarPath(string fileName);

        public PathPair GetViewTemplatePath(string fileName);

        public PathPair GetTestPath(string fileName);
    }
}