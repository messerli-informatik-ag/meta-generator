using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Messerli.BackbonePluginTemplatePlugin.Variants.ViewPlugin
{
    public class PluginVariant : IPluginVariant
    {
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
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.ProjectFile, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, $"{_templateFileProperty.PluginName}.csproj"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.Plugin, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, "Plugin.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.Module, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, "Module.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.AssemblyInfo, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, "AssemblyInfo.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.IPresenter, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "IPresenter.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.Presenter, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "Presenter.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.IView, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "IView.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.View, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "View.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.ResponseRenderer, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "ResponseRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.ViewModel, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, ViewFolder, "ViewModel.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.IController, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, "IController.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.Controller, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, GuiFolder, "Controller.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.EndpointConstant, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, RegistrarFolder, "EndpointConstant.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.INavigationRegistrar, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, RegistrarFolder, "INavigationRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.NavigationRegistrar, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, RegistrarFolder, "NavigationRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.IRouteRegistrar, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, RegistrarFolder, "IRouteRenderer.cs"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.RouteRegistrar, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, RegistrarFolder, "RouteRegistrar.cs"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.PluginTemplateMustache, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, TemplateFolder, "PluginTemplate.mustache"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.Icon, Path.Combine(_templateFileProperty.SolutionDirectory, _templateFileProperty.PluginName, PublicFolder, IconsFolder, "hello-world.svg"), Encoding.UTF8),

                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.TestProjectFile, Path.Combine(_templateFileProperty.SolutionDirectory, $"{_templateFileProperty.PluginName}.{TestFolder}", $"{_templateFileProperty.PluginName}.csproj"), Encoding.UTF8),
                _templateFileProperty.FileGenerator.FromTemplate(ViewPlugin.Template.IntegrationTestSource, Path.Combine(_templateFileProperty.SolutionDirectory, $"{_templateFileProperty.PluginName}.{TestFolder}", "IntegrationTests.cs"), Encoding.UTF8),
            };
    }
}
