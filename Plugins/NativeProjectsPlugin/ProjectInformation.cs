using System;
using System.IO;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.NativeProjectsPlugin
{
    internal class ProjectInformation : IProjectInformation
    {
        private readonly IUserInputProvider _userInputProvider;
        private Guid _guid = Guid.NewGuid();

        public ProjectInformation(IUserInputProvider userInputProvider)
        {
            _userInputProvider = userInputProvider;
        }

        public string ProjectGuid => _guid.ToString();

        public string HeaderPath(string fileName)
        {
            throw new NotImplementedException();
        }

        public string ProjectPath(string fileName) => Path.Combine(ProjectPath(), fileName);

        public string SourcePath(string fileName)
        {
            throw new NotImplementedException();
        }

        public string ProjectPath()
        {
            var branchPath = _userInputProvider.Value(Variable.Branch);
            var subfolder = _userInputProvider.Value(Variable.Subfolder);
            var projectName = _userInputProvider.Value(Variable.ProjectName);

            return subfolder.Length == 0
                ? Path.Combine(branchPath, projectName)
                : Path.Combine(branchPath, subfolder, projectName);
        }

        public string PropertyPath(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
