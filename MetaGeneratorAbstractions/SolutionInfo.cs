using System;

namespace Messerli.MetaGeneratorAbstractions
{
    public class SolutionInfo
    {
        private SolutionInfo(string path, string? filterFolder)
        {
            Path = path;
            FilterFolder = filterFolder;
        }

        public string Path { get; }

        public string? FilterFolder { get; }

        public class Builder
        {
            private string _path = string.Empty;
            private string? _filterFolder = null;

            public Builder()
            {
            }

            public Builder WithPath(string path)
            {
                _path = path;
                return this;
            }

            public Builder WithFilterFolder(string filterFolder)
            {
                _filterFolder = filterFolder;
                return this;
            }

            public SolutionInfo Build()
            {
                if (_path.Length == 0)
                {
                    throw new ArgumentException(nameof(_path));
                }

                return new SolutionInfo(_path, _filterFolder);
            }
        }
    }
}