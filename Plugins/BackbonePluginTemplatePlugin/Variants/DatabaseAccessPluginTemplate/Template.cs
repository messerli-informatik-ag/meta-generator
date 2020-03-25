namespace Messerli.BackbonePluginTemplatePlugin.Variants.DatabaseAccessPluginTemplate
{
    internal static class Template
    {
        public static string ProjectFile => WithPath("Project.csproj.template");

        public static string Plugin => WithPath("Plugin.Source.template");

        public static string Module => WithPath("Module.Source.template");

        public static string AssemblyInfo => WithPath("AssemblyInfo.Source.template");

        public static string IPersonPersistence => WithSubfolderDatabasePath("IPersonPersistence.Source.template");

        public static string PersonPersistence => WithSubfolderDatabasePath("PersonPersistence.Source.template");

        public static string Person => WithSubfolderDatabasePath("Person.Source.template");

        public static string IPresenter => WithSubfolderGuiViewPath("IPresenter.Source.template");

        public static string Presenter => WithSubfolderGuiViewPath("Presenter.Source.template");

        public static string IView => WithSubfolderGuiViewPath("IView.Source.template");

        public static string View => WithSubfolderGuiViewPath("View.Source.template");

        public static string PersonView => WithSubfolderGuiViewPath("Person.Source.template");

        public static string ResponseRenderer => WithSubfolderGuiViewPath("ResponseRenderer.Source.template");

        public static string ViewModel => WithSubfolderGuiViewPath("ViewModel.Source.template");

        public static string Controller => WithSubfolderGuiPath("Controller.Source.template");

        public static string IController => WithSubfolderGuiPath("IController.Source.template");

        public static string MigrationSql => WithSubfolderMigrationsPath("0_create_database_access_plugin_template_entry.Source.template");

        public static string Icon => WithSubfolderPublicIconPath("hello-world.Source.template");

        public static string EndpointConstant => WithSubfolderRegistrarPath("EndpointConstant.Source.template");

        public static string INavigationRegistrar => WithSubfolderRegistrarPath("INavigationRegistrar.Source.template");

        public static string IRouteRegistrar => WithSubfolderRegistrarPath("IRouteRegistrar.Source.template");

        public static string NavigationRegistrar => WithSubfolderRegistrarPath("NavigationRegistrar.Source.template");

        public static string RouteRegistrar => WithSubfolderRegistrarPath("RouteRegistrar.Source.template");

        public static string PluginTemplateMustache => WithSubfolderTemplatesPath("PluginTemplate.Source.template");

        public static string TestProjectFile => WithTestPath("Project.Test.csproj.template");

        public static string IntegrationTestSource => WithTestPath("IntegrationTests.Source.template");

        private static string WithPath(string rest) => $"Messerli.BackbonePluginTemplatePlugin.templates.DatabaseAccessPluginTemplate.{rest}";

        private static string WithSubfolderDatabasePath(string rest) => WithPath($"Database.{rest}");

        private static string WithSubfolderMigrationsPath(string rest) => WithPath($"Migrations.{rest}");

        private static string WithSubfolderGuiPath(string rest) => WithPath($"Gui.{rest}");

        private static string WithSubfolderGuiViewPath(string rest) => WithPath($"Gui.View.{rest}");

        private static string WithSubfolderPublicIconPath(string rest) => WithPath($"Public.icons.{rest}");

        private static string WithSubfolderRegistrarPath(string rest) => WithPath($"Registrar.{rest}");

        private static string WithSubfolderTemplatesPath(string rest) => WithPath($"Templates.{rest}");

        private static string WithTestPath(string rest) => WithPath($"Test.{rest}");
    }
}
