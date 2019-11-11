using System;
using System.IO;
using System.Linq;
using Funcky.Monads;
using Messerli.ProjectAbstractions;

namespace Messerli.ProjectGenerator
{
    public class TemplateLoader : ITemplateLoader
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
                return FindTemplate(templateName).Match(
                    none: () => throw new NotImplementedException(),
                    some: s => s);
            }

            throw new Exception($"There is no template resource with the name {templateName}");
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
            using (var templateStream = GetTemplateStream(templateName))
            {
                return templateStream != null
                    ? Option.Some(ReadTemplate(templateStream))
                    : Option<string>.None();
            }
        }

        private string ReadTemplate(Stream templateStream)
        {
            using (StreamReader reader = new StreamReader(templateStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}