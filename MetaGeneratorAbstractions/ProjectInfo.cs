using System;
using System.IO;
using Messerli.VsSolution.Model;

namespace Messerli.MetaGeneratorAbstractions
{
    public class ProjectInfo
    {
        private ProjectInfo(string name, string path, Guid? guid, ProjectType.Identifier type)
        {
            Name = name;
            Path = path;
            Guid = guid;
            Type = type;
        }

        public string Name { get; }

        public string Path { get; }

        public Guid? Guid { get; }

        public ProjectType.Identifier Type { get; }

        public class Builder
        {
            private string _name = string.Empty;
            private string _path = string.Empty;
            private Guid? _guid = null;
            private ProjectType.Identifier _type = ProjectType.Identifier.CSharpSdk;

            public Builder()
            {
            }

            public ProjectInfo Build()
            {
                if (_name.Length == 0)
                {
                    throw new ArgumentException(nameof(_name));
                }

                if (File.Exists(_path) == false)
                {
                    throw new ArgumentException(nameof(_path));
                }

                return new ProjectInfo(_name, _path, _guid, _type);
            }

            public Builder WithName(string name)
            {
                _name = name;
                return this;
            }

            public Builder WithPath(string path)
            {
                _path = path;
                return this;
            }

            public Builder WithGuid(Guid? guid)
            {
                _guid = guid;
                return this;
            }

            public Builder WithType(ProjectType.Identifier type)
            {
                _type = type;
                return this;
            }
        }
    }
}