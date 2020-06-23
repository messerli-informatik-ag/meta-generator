namespace Messerli.FileManipulatorAbstractions.Project
{
    /// <summary>
    /// Source: <a href="https://docs.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files">docs.microsoft.com</a>.
    /// </summary>
    public enum DependencyAssetName
    {
        Compile,
        Runtime,
        ContentFiles,
        Build,
        BuildMultiTargeting,
        BuildTransitive,
        Analyzers,
        Native,
    }
}