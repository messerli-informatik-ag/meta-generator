namespace Messerli.FileManipulatorAbstractions
{
    public interface IPackageSources
    {
        void List(string configFile, string format);

        void Add(string configFile, NugetPackageSource packageSource);

        void Update(string configFile, NugetPackageSource packageSource);

        void Remove(string configFile, string name);

        void Enable(string configFile, string name);

        void Disable(string configFile, string name);
    }
}