using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Messerli.BackbonePluginTemplatePlugin.Variants.TemplateConstant;

namespace Messerli.BackbonePluginTemplatePlugin.Variants.DatabaseAccessPlugin
{
    public class PluginVariant : IPluginVariant
    {
        private readonly TemplateFileProperty _templateFileProperty;

        public PluginVariant(TemplateFileProperty templateFileProperty)
        {
            _templateFileProperty = templateFileProperty;
        }

        private string DatabasePath => Path.Combine(_templateFileProperty.PluginPath, DatabasePath);

        private string GuiPath => Path.Combine(_templateFileProperty.PluginPath, GuiFolder);

        private string ViewPath => Path.Combine(GuiPath, ViewFolder);

        private string RegistrarPath => Path.Combine(_templateFileProperty.PluginPath, RegistrarFolder);

        private string ViewTemplatePath => Path.Combine(_templateFileProperty.PluginPath, TemplateFolder);

        private string IconPath => Path.Combine(_templateFileProperty.PluginPath, PublicFolder, IconsFolder);

        private string MigrationPath => Path.Combine(_templateFileProperty.PluginPath, MigrationFolder);

        public List<Task> CreateTemplateFiles()
            => new List<Task>
            {
                FromTemplate(Template.ProjectFile, new[] { _templateFileProperty.PluginPath, $"{_templateFileProperty.PluginName}.csproj" }),
                FromTemplate(Template.Plugin, new[] { _templateFileProperty.PluginPath, "Plugin.cs" }),
                FromTemplate(Template.Module, new[] { _templateFileProperty.PluginPath, "Module.cs" }),
                FromTemplate(Template.AssemblyInfo, new[] { _templateFileProperty.PluginPath, "AssemblyInfo.cs" }),

                FromTemplate(Template.IPersonPersistence, new[] { DatabasePath, "IPersonPersistence.cs" }),
                FromTemplate(Template.PersonPersistence, new[] { DatabasePath, "PersonPersistence.cs" }),
                FromTemplate(Template.Person, new[] { DatabasePath, "Person.cs" }),

                FromTemplate(Template.IPresenter, new[] { ViewPath, "IPresenter.cs" }),
                FromTemplate(Template.Presenter, new[] { ViewPath, "Presenter.cs" }),
                FromTemplate(Template.IView, new[] { ViewPath, "IView.cs" }),
                FromTemplate(Template.View, new[] { ViewPath, "View.cs" }),
                FromTemplate(Template.ResponseRenderer, new[] { ViewPath, "ResponseRenderer.cs" }),
                FromTemplate(Template.ViewModel, new[] { ViewPath, "ViewModel.cs" }),
                FromTemplate(Template.PersonView, new[] { ViewPath, "Person.cs" }),

                FromTemplate(Template.MigrationSql, new[] { MigrationPath, "0_create_database_access_plugin_template_entry.sql" }),

                FromTemplate(Template.IController, new[] { GuiPath, "IController.cs" }),
                FromTemplate(Template.Controller, new[] { GuiPath, "Controller.cs" }),

                FromTemplate(Template.EndpointConstant, new[] { RegistrarPath, "EndpointConstant.cs" }),
                FromTemplate(Template.INavigationRegistrar, new[] { RegistrarPath, "INavigationRenderer.cs" }),
                FromTemplate(Template.NavigationRegistrar, new[] { RegistrarPath, "NavigationRenderer.cs" }),
                FromTemplate(Template.IRouteRegistrar, new[] { RegistrarPath, "IRouteRenderer.cs" }),
                FromTemplate(Template.RouteRegistrar, new[] { RegistrarPath, "RouteRegistrar.cs" }),

                FromTemplate(Template.PluginTemplateMustache, new[] { ViewTemplatePath, "PluginTemplate.mustache" }),

                FromTemplate(Template.Icon, new[] { IconPath, "hello-world.svg" }),

                FromTemplate(Template.TestProjectFile, new[] { _templateFileProperty.SolutionDirectory, $"{_templateFileProperty.PluginName}.{TestFolder}", $"{_templateFileProperty.PluginName}.csproj" }),
                FromTemplate(Template.IntegrationTestSource, new[] { _templateFileProperty.SolutionDirectory, $"{_templateFileProperty.PluginName}.{TestFolder}", "IntegrationTests.cs" }),
            };

        private Task FromTemplate(string templatePath, IEnumerable<string> relativePathToResult)
            => _templateFileProperty.FileGenerator.FromTemplate(Template.PersonView, CombinePathEnumerable(relativePathToResult), Encoding.UTF8);

        private static string CombinePathEnumerable(IEnumerable<string> paths)
            => paths.Aggregate(string.Empty, Path.Combine);
    }
}
