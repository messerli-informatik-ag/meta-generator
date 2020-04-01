using System;
using System.Collections.Generic;

namespace Messerli.FileManipulatorAbstractions
{
    public class GlobalJsonModificationBuilder
    {
        private readonly List<MsBuildSdk> _sdkList = new List<MsBuildSdk>();
        private string _path;

        private GlobalJsonModificationBuilder(string path)
        {
            _path = path;
        }

        public GlobalJsonModificationBuilder WithPath(string path)
            => new GlobalJsonModificationBuilder(path);

        public void AddSdk(MsBuildSdk sdk)
            => _sdkList.Add(sdk);

        public GlobalJsonModification Build()
        {
            if (_path.Length == 0)
            {
                throw new ArgumentException(nameof(_path));
            }

            return new GlobalJsonModification(_path, _sdkList);
        }
    }
}
