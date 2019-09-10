namespace Messerli.ProjectAbstractions
{
    public interface IProjectGenerator
    {
        string Name { get; }

        string ShortName { get; }

        void Generate();
    }
}
