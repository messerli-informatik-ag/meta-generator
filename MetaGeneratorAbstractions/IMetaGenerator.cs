namespace Messerli.MetaGeneratorAbstractions
{
    public interface IMetaGenerator
    {
        string Description { get; }

        string Name { get; }

        /// <summary>
        /// Registers all necessary input from the user.
        /// </summary>
        void Register();

        /// <summary>
        /// Steps which are necessary before the file generation/alteration. (checkout files from the repository.)
        /// </summary>
        void Prepare();

        /// <summary>
        /// Generate the files.
        /// </summary>
        void Generate();

        /// <summary>
        /// Steps which are necessary after the file generation. (adding files to repository or cleanup.)
        /// </summary>
        void TearDown();
    }
}
