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

        public List<Task> CreateTemplateFiles()
            => new List<Task>
            {
                FromTemplate(Template.ProjectFile, new[] { _templateFileProperty.PluginPath, $"{_templateFileProperty.PluginName}.csproj" }),
                FromTemplate(Template.Plugin, new[] { _templateFileProperty.PluginPath, "Plugin.cs" }),
                FromTemplate(Template.Module, new[] { _templateFileProperty.PluginPath, "Module.cs" }),
                FromTemplate(Template.AssemblyInfo, new[] { _templateFileProperty.PluginPath, "AssemblyInfo.cs" }),

                FromTemplate(Template.IPersonPersistence, new[] { _templateFileProperty.PluginPath, DatabaseFolder, "IPersonPersistence.cs" }),
                FromTemplate(Template.PersonPersistence, new[] { _templateFileProperty.PluginPath, DatabaseFolder, "PersonPersistence.cs" }),
                FromTemplate(Template.Person, new[] { _templateFileProperty.PluginPath, DatabaseFolder, "Person.cs" }),

                FromTemplate(Template.IPresenter, new[] { _templateFileProperty.PluginPath, GuiFolder, ViewFolder, "IPresenter.cs" }),
                FromTemplate(Template.Presenter, new[] { _templateFileProperty.PluginPath, GuiFolder, ViewFolder, "Presenter.cs" }),
                FromTemplate(Template.IView, new[] { _templateFileProperty.PluginPath, GuiFolder, ViewFolder, "IView.cs" }),
                FromTemplate(Template.View, new[] { _templateFileProperty.PluginPath, GuiFolder, ViewFolder, "View.cs" }),
                FromTemplate(Template.ResponseRenderer, new[] { _templateFileProperty.PluginPath, GuiFolder, ViewFolder, "ResponseRenderer.cs" }),
                FromTemplate(Template.ViewModel, new[] { _templateFileProperty.PluginPath, GuiFolder, ViewFolder, "ViewModel.cs" }),
                FromTemplate(Template.PersonView, new[] { _templateFileProperty.PluginPath, GuiFolder, ViewFolder, "Person.cs" }),

                FromTemplate(Template.MigrationSql, new[] { _templateFileProperty.PluginPath, GuiFolder, ViewFolder, "0_create_database_access_plugin_template_entry.sql" }),

                FromTemplate(Template.IController, new[] { _templateFileProperty.PluginPath, GuiFolder, "IController.cs" }),
                FromTemplate(Template.Controller, new[] { _templateFileProperty.PluginPath, GuiFolder, "Controller.cs" }),

                FromTemplate(Template.EndpointConstant, new[] { _templateFileProperty.PluginPath, RegistrarFolder, "EndpointConstant.cs" }),
                FromTemplate(Template.INavigationRegistrar, new[] { _templateFileProperty.PluginPath, RegistrarFolder, "INavigationRenderer.cs" }),
                FromTemplate(Template.NavigationRegistrar, new[] { _templateFileProperty.PluginPath, RegistrarFolder, "NavigationRenderer.cs" }),
                FromTemplate(Template.IRouteRegistrar, new[] { _templateFileProperty.PluginPath, RegistrarFolder, "IRouteRenderer.cs" }),
                FromTemplate(Template.RouteRegistrar, new[] { _templateFileProperty.PluginPath, RegistrarFolder, "RouteRegistrar.cs" }),

                FromTemplate(Template.PluginTemplateMustache, new[] { _templateFileProperty.PluginPath, TemplateFolder, "PluginTemplate.mustache" }),

                FromTemplate(Template.Icon, new[] { _templateFileProperty.PluginPath, PublicFolder, IconsFolder, "hello-world.svg" }),

                FromTemplate(Template.TestProjectFile, new[] { _templateFileProperty.SolutionDirectory, $"{_templateFileProperty.PluginName}.{TestFolder}", $"{_templateFileProperty.PluginName}.csproj" }),
                FromTemplate(Template.IntegrationTestSource, new[] { _templateFileProperty.SolutionDirectory, $"{_templateFileProperty.PluginName}.{TestFolder}", "IntegrationTests.cs" }),
            };

        private Task FromTemplate(string templatePath, IEnumerable<string> relativePathToResult)
            => _templateFileProperty.FileGenerator.FromTemplate(Template.PersonView, CombinePathEnumerable(relativePathToResult), Encoding.UTF8);

        private static string CombinePathEnumerable(IEnumerable<string> paths)
            => paths.Aggregate(string.Empty, Path.Combine);
    }
}
