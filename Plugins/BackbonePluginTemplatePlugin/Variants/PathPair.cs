namespace Messerli.BackbonePluginTemplatePlugin.Variants
{
    internal readonly struct PathPair
    {
        public readonly string ResultPath;

        private readonly string _templatePath;

        public PathPair(string templatePath, string resultPath)
        {
            _templatePath = templatePath;
            ResultPath = resultPath;
        }

        public string TemplatePath => _templatePath + ".mustache";
    }
}