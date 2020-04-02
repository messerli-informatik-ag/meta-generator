using System;

namespace Messerli.BackbonePluginTemplatePlugin.Variants
{
    internal class PathPairRetriever : IPathPairRetriever
    {
        private readonly IPathRetriever _templatePathRetriever;
        private readonly IPathRetriever _resultPathRetriever;

        public PathPairRetriever(IPathRetriever templatePathRetriever, IPathRetriever resultPathRetriever)
        {
            _templatePathRetriever = templatePathRetriever;
            _resultPathRetriever = resultPathRetriever;
        }

        public PathPair GetPluginPath(string fileName)
            => GetPathPair(pathRetriever => pathRetriever.GetPluginPath(fileName));

        public PathPair GetDatabasePath(string fileName)
            => GetPathPair(pathRetriever => pathRetriever.GetDatabasePath(fileName));

        public PathPair GetMigrationsPath(string fileName)
            => GetPathPair(pathRetriever => pathRetriever.GetMigrationsPath(fileName));

        public PathPair GetGuiPath(string fileName)
            => GetPathPair(pathRetriever => pathRetriever.GetGuiPath(fileName));

        public PathPair GetViewPath(string fileName)
            => GetPathPair(pathRetriever => pathRetriever.GetViewPath(fileName));

        public PathPair GetIconPath(string fileName)
            => GetPathPair(pathRetriever => pathRetriever.GetIconPath(fileName));

        public PathPair GetRegistrarPath(string fileName)
            => GetPathPair(pathRetriever => pathRetriever.GetRegistrarPath(fileName));

        public PathPair GetViewTemplatePath(string fileName)
            => GetPathPair(pathRetriever => pathRetriever.GetViewTemplatePath(fileName));

        public PathPair GetTestPath(string fileName)
            => GetPathPair(pathRetriever => pathRetriever.GetTestPath(fileName));

        private PathPair GetPathPair(Func<IPathRetriever, string> retrievalFunc)
            => new PathPair(retrievalFunc(_templatePathRetriever), retrievalFunc(_resultPathRetriever));
    }
}