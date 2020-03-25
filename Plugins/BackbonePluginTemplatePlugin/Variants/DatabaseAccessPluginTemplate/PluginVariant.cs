using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Messerli.BackbonePluginTemplatePlugin.Variants.DatabaseAccessPluginTemplate
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
                _templateFileProperty.FileGenerator.FromTemplate(Template.ProjectFile, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, $"{_templateFileProperty.RepositoryName}.csproj"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.Plugin, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, "Plugin.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.Module, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, "Module.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.AssemblyInfo, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, "AssemblyInfo.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(Template.IPersonPersistence, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, DatabaseFolder, "IPersonPersistence.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.PersonPersistence, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, DatabaseFolder, "PersonPersistence.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.Person, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, DatabaseFolder, "Person.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(Template.IPresenter, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, GuiFolder, ViewFolder, "IPresenter.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.Presenter, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, GuiFolder, ViewFolder, "Presenter.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.IView, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, GuiFolder, ViewFolder, "IView.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.View, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, GuiFolder, ViewFolder, "View.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.ResponseRenderer, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, GuiFolder, ViewFolder, "ResponseRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.ViewModel, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, GuiFolder, ViewFolder, "ViewModel.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.PersonView, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, GuiFolder, ViewFolder, "Person.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(Template.MigrationSql, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, GuiFolder, ViewFolder, "0_create_database_access_plugin_template_entry.sql"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(Template.IController, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, GuiFolder, "IController.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.Controller, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, GuiFolder, "Controller.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(Template.EndpointConstant, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, RegistrarFolder, "EndpointConstant.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.INavigationRegistrar, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, RegistrarFolder, "INavigationRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.NavigationRegistrar, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, RegistrarFolder, "NavigationRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.IRouteRegistrar, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, RegistrarFolder, "IRouteRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.RouteRegistrar, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, RegistrarFolder, "RouteRegistrar.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(Template.PluginTemplateMustache, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, TemplateFolder, "PluginTemplate.mustache"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(Template.Icon, Path.Combine(_templateFileProperty.RepositoryPath, _templateFileProperty.RepositoryName, PublicFolder, IconsFolder, "hello-world.svg"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(Template.TestProjectFile, Path.Combine(_templateFileProperty.RepositoryPath, $"{_templateFileProperty.RepositoryName}.{TestFolder}", $"{_templateFileProperty.RepositoryName}.csproj"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(Template.IntegrationTestSource, Path.Combine(_templateFileProperty.RepositoryPath, $"{_templateFileProperty.RepositoryName}.{TestFolder}", "IntegrationTests.cs"), Encoding.UTF8),
            };
    }
}
