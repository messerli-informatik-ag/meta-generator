namespace Messerli.MetaGeneratorAbstractions
{
    public interface IDotnetToolInstaller
    {
        void InstallTool(string path, string toolName, string? version);
    }
}
