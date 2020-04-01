namespace Messerli.BackbonePluginTemplatePlugin.Variants.DatabaseAccessPlugin
{
    internal static class Template
    {
        public static string ProjectFile => GetPath("Project.csproj.template");

        public static string Plugin => GetPath("Plugin.Source.template");

        public static string Module => GetPath("Module.Source.template");

        public static string AssemblyInfo => GetPath("AssemblyInfo.Source.template");

        public static string IPersonPersistence => GetDatabasePath("IPersonPersistence.Source.template");

        public static string PersonPersistence => GetDatabasePath("PersonPersistence.Source.template");

        public static string Person => GetDatabasePath("Person.Source.template");

        public static string IPresenter => GetViewPath("IPresenter.Source.template");

        public static string Presenter => GetViewPath("Presenter.Source.template");

        public static string IView => GetViewPath("IView.Source.template");

        public static string View => GetViewPath("View.Source.template");

        public static string PersonView => GetViewPath("Person.Source.template");

        public static string ResponseRenderer => GetViewPath("ResponseRenderer.Source.template");

        public static string ViewModel => GetViewPath("ViewModel.Source.template");

        public static string Controller => GetGuiPath("Controller.Source.template");

        public static string IController => GetGuiPath("IController.Source.template");

        public static string MigrationSql => GetMigrationsPath("0_create_database_access_plugin_template_entry.Source.template");

        public static string Icon => GetIconPath("hello-world.Source.template");

        public static string EndpointConstant => GetRegistrarPath("EndpointConstant.Source.template");

        public static string INavigationRegistrar => GetRegistrarPath("INavigationRegistrar.Source.template");

        public static string IRouteRegistrar => GetRegistrarPath("IRouteRegistrar.Source.template");

        public static string NavigationRegistrar => GetRegistrarPath("NavigationRegistrar.Source.template");

        public static string RouteRegistrar => GetRegistrarPath("RouteRegistrar.Source.template");

        public static string PluginTemplateMustache => GetViewTemplatePath("PluginTemplate.Source.template");

        public static string TestProjectFile => GetTestPath("Project.Test.csproj.template");

        public static string IntegrationTestSource => GetTestPath("IntegrationTests.Source.template");

        private static string GetPath(string rest) => $"Messerli.BackbonePluginTemplatePlugin.templates.DatabaseAccessPluginTemplate.{rest}";

        private static string GetDatabasePath(string rest) => GetPath($"Database.{rest}");

        private static string GetMigrationsPath(string rest) => GetPath($"Migrations.{rest}");

        private static string GetGuiPath(string rest) => GetPath($"Gui.{rest}");

        private static string GetViewPath(string rest) => GetPath($"Gui.View.{rest}");

        private static string GetIconPath(string rest) => GetPath($"Public.icons.{rest}");

        private static string GetRegistrarPath(string rest) => GetPath($"Registrar.{rest}");

        private static string GetViewTemplatePath(string rest) => GetPath($"Templates.{rest}");

        private static string GetTestPath(string rest) => GetPath($"Test.{rest}");
    }
}
