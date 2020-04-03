namespace Messerli.FileManipulatorAbstractions
{
    public interface INugetPackageSourceManipulator
    {
        void Add(string configFile, NugetPackageSource packageSource);

        void Update(string configFile, NugetPackageSource packageSource);

        void Remove(string configFile, string name);

        void Enable(string configFile, string name);

        void Disable(string configFile, string name);
    }
}
