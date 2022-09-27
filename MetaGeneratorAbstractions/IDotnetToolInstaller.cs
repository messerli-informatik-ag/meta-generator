namespace Messerli.MetaGeneratorAbstractions;

public interface IDotnetToolInstaller
{
    void RegisterTool();

    void InstallTool(string path, string toolName);

    void InstallTool(string path, string toolName, string version);
}
