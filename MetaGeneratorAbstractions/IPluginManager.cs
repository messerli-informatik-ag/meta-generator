namespace Messerli.MetaGenerator
{
    public interface IPluginManager
    {
        int Install(string pluginName);

        int Uninstall(string pluginName);
    }
}