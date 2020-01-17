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
        /// Steps which are necessary before the file generation/alteration. (adding files to repository, cleanup ...)
        /// </summary>
        void Prepare();

        /// <summary>
        /// Generate the project.
        /// </summary>
        void Generate();

        /// <summary>
        /// Steps which are necessary after the file generation. (adding files to repository, cleanup ...)
        /// </summary>
        void TearDown();
    }
}
