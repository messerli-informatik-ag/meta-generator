using System;
using Messerli.FileManipulatorAbstractions;

namespace Messerli.FileManipulator
{
    public sealed class NugetPackageSources : INugetPackageSources
    {
        public void List(string configFile, string format)
            => throw new NotImplementedException();

        public void Add(string configFile, NugetPackageSource packageSource)
            => throw new NotImplementedException();

        public void Update(string configFile, NugetPackageSource packageSource)
            => throw new NotImplementedException();

        public void Remove(string configFile, string name)
            => throw new NotImplementedException();

        public void Enable(string configFile, string name)
            => throw new NotImplementedException();

        public void Disable(string configFile, string name)
            => throw new NotImplementedException();
    }
}