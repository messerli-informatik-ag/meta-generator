using System;
using System.Collections.Generic;

namespace Messerli.FileManipulatorAbstractions
{
    public class MsBuildSdkBuilder
    {
        private readonly List<MsBuildSdk> _sdkList = new List<MsBuildSdk>();
        private string _path;

        private MsBuildSdkBuilder(string path)
        {
            _path = path;
        }

        public MsBuildSdkBuilder WithPath(string path)
            => new MsBuildSdkBuilder(path);

        public void AddSdk(MsBuildSdk sdk)
            => _sdkList.Add(sdk);

        public MsBuildSdkInfo Build()
        {
            if (_path.Length == 0)
            {
                throw new ArgumentException(nameof(_path));
            }

            return new MsBuildSdkInfo(_path, _sdkList);
        }
    }
}
