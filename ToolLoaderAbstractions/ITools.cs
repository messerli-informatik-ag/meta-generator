namespace Messerli.ToolLoaderAbstractions
{
    public interface ITools
    {
        public ITool CreateTool(string name, string executable);
    }
}
