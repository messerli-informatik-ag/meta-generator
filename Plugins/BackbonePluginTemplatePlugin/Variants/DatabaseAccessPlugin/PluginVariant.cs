using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messerli.MetaGeneratorAbstractions;
using static Messerli.BackbonePluginTemplatePlugin.Variants.TemplateConstant;

namespace Messerli.BackbonePluginTemplatePlugin.Variants.DatabaseAccessPlugin
{
    public class PluginVariant : IPluginVariant
    {
        private readonly IPathPairRetriever _pathPairRetriever;
        private readonly IFileGenerator _fileGenerator;
        private readonly string _pluginName;

        public PluginVariant(TemplateFileProperty templateFileProperty)
        {
            _pluginName = templateFileProperty.PluginName;
            _templatePathRetriever =
                new PathRetriever("Messerli.BackbonePluginTemplatePlugin.templates", _pluginName, ".");
            _resultPathRetriever = new PathRetriever(templateFileProperty.SolutionDirectory, templateFileProperty.PluginName, Path.DirectorySeparatorChar.ToString());
        }

        public List<Task> CreateTemplateFiles()
            => GetTemplateResultPairs()
                .Select(templateResultPair =>
                    _fileGenerator.FromTemplate(templateResultPair.TemplatePath, templateResultPair.ResultPath, Encoding.UTF8))
                .ToList();

        private IEnumerable<PathPair> GetTemplateResultPairs()
            => new[]
            {
                _pathPairRetriever.GetPluginPath($"{_pluginName}.csproj"),
                _pathPairRetriever.GetPluginPath("Plugin.cs"),
                _pathPairRetriever.GetPluginPath("Module.cs"),
                _pathPairRetriever.GetPluginPath("AssemblyInfo.cs"),

                _pathPairRetriever.GetDatabasePath("IPersonPersistence.cs"),
                _pathPairRetriever.GetDatabasePath("PersonPersistence.cs"),
                _pathPairRetriever.GetDatabasePath("Person.cs"),

                _pathPairRetriever.GetViewPath("IPresenter.cs"),
                _pathPairRetriever.GetViewPath("Presenter.cs"),
                _pathPairRetriever.GetViewPath("IView.cs"),
                _pathPairRetriever.GetViewPath("View.cs"),
                _pathPairRetriever.GetViewPath("ResponseRenderer.cs"),
                _pathPairRetriever.GetViewPath("ViewModel.cs"),
                _pathPairRetriever.GetViewPath("Person.cs"),

                _pathPairRetriever.GetMigrationsPath("0_create_database_access_plugin_template_entry.sql"),

                _pathPairRetriever.GetGuiPath("IController.cs"),
                _pathPairRetriever.GetGuiPath("Controller.cs"),

                _pathPairRetriever.GetRegistrarPath("EndpointConstant.cs"),
                _pathPairRetriever.GetRegistrarPath("INavigationRenderer.cs"),
                _pathPairRetriever.GetRegistrarPath("NavigationRenderer.cs"),
                _pathPairRetriever.GetRegistrarPath("IRouteRenderer.cs"),
                _pathPairRetriever.GetRegistrarPath("RouteRegistrar.cs"),

                _pathPairRetriever.GetViewTemplatePath("PluginTemplate.mustache"),

                _pathPairRetriever.GetIconPath("hello-world.svg"),

                _pathPairRetriever.GetTestPath($"{_pluginName}.csproj"),
                _pathPairRetriever.GetTestPath("IntegrationTests.csproj"),
            };
    }
}
