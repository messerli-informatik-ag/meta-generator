using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNet.Globbing;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator
{
    internal class TemplateLoader : ITemplateLoader
    {
        private readonly IExecutingPluginAssemblyProvider _assemblyProvider;

        public TemplateLoader(IExecutingPluginAssemblyProvider assemblyProvider)
        {
            _assemblyProvider = assemblyProvider;
        }

        public string GetTemplate(string templateName)
        {
            if (_assemblyProvider.PluginAssembly.GetManifestResourceNames().Contains(templateName))
            {
                return FindTemplate(templateName).GetOrElse(
                    () => throw new NotImplementedException());
            }

            throw new Exception($"There is no template resource with the name {templateName}");
        }

        public IEnumerable<Template> GetTemplatesFromGlob(string glob)
        {
            var globMatcher = Glob.Parse(glob);
            return _assemblyProvider.PluginAssembly
                .GetManifestResourceNames()
                .Where(resourceName => globMatcher.IsMatch(resourceName))
                .Select(templateName => new Template(templateName, GetTemplate(templateName)));
        }

        public Stream? GetTemplateStream(string templateName)
        {
            if (_assemblyProvider.PluginAssembly.GetManifestResourceNames().Contains(templateName))
            {
                return _assemblyProvider.PluginAssembly.GetManifestResourceStream(templateName);
            }

            throw new Exception($"There is no template resource with the name {templateName}");
        }

        private Option<string> FindTemplate(string templateName)
        {
            using var templateStream = GetTemplateStream(templateName);

            return Option.FromNullable(templateStream).Select(ReadTemplate);
        }

        private string ReadTemplate(Stream templateStream)
        {
            using var reader = new StreamReader(templateStream);

            return reader.ReadToEnd();
        }
    }
}
