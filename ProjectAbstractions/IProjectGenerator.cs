namespace Messerli.ProjectAbstractions
{
    public interface IProjectGenerator
    {
        string Name { get; }

        string ShortName { get; }

        /// <summary>
        /// Registers all necessary input from the user.
        /// </summary>
        void Register();

        /// <summary>
        /// Generate the project.
        /// </summary>
        void Generate();
    }
}
