using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Messerli.BackbonePluginTemplatePlugin.Variants.DatabaseAccessPlugin
{
    public class PluginVariant : IPluginVariant
    {
        private const string DatabaseFolder = "Database";
        private const string GuiFolder = "Gui";
        private const string ViewFolder = "View";
        private const string PublicFolder = "Public";
        private const string IconsFolder = "icons";
        private const string RegistrarFolder = "Registrar";
        private const string TemplateFolder = "Templates";
        private const string TestFolder = "Test";

        private readonly TemplateFileProperty _templateFileProperty;

        public PluginVariant(TemplateFileProperty templateFileProperty)
        {
            _templateFileProperty = templateFileProperty;
        }

        public List<Task> CreateTemplateFiles()
            => new List<Task>
            {
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.ProjectFile, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, $"{_templateFileProperty.PluginName}.csproj"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.Plugin, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, "Plugin.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.Module, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, "Module.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.AssemblyInfo, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, "AssemblyInfo.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.IPersonPersistence, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, DatabaseFolder, "IPersonPersistence.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.PersonPersistence, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, DatabaseFolder, "PersonPersistence.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.Person, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, DatabaseFolder, "Person.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.IPresenter, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "IPresenter.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.Presenter, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "Presenter.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.IView, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "IView.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.View, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "View.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.ResponseRenderer, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "ResponseRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.ViewModel, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "ViewModel.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.PersonView, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "Person.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.MigrationSql, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "0_create_database_access_plugin_template_entry.sql"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.IController, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, "IController.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.Controller, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, "Controller.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.EndpointConstant, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, RegistrarFolder, "EndpointConstant.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.INavigationRegistrar, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, RegistrarFolder, "INavigationRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.NavigationRegistrar, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, RegistrarFolder, "NavigationRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.IRouteRegistrar, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, RegistrarFolder, "IRouteRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.RouteRegistrar, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, RegistrarFolder, "RouteRegistrar.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.PluginTemplateMustache, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, TemplateFolder, "PluginTemplate.mustache"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.Icon, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, PublicFolder, IconsFolder, "hello-world.svg"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.TestProjectFile, Path.Combine(_templateFileProperty.SolutionDirectory, $"{_templateFileProperty.PluginName}.{TestFolder}", $"{_templateFileProperty.PluginName}.csproj"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(DatabaseAccessPlugin.Template.IntegrationTestSource, Path.Combine(_templateFileProperty.SolutionDirectory, $"{_templateFileProperty.PluginName}.{TestFolder}", "IntegrationTests.cs"), Encoding.UTF8),
            };
    }
}
