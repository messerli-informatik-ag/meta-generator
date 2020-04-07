using System.Collections.Generic;
using System.Linq;

namespace Messerli.FileManipulator.Project.MsBuild
{
    internal sealed class ProjectSdkManipulator : IProjectSdkManipulator
    {
        public void AddSdksToProject(Microsoft.Build.Evaluation.Project project, IEnumerable<string> sdksToAdd)
        {
            var existingSdks = ParseSdkList(project.Xml.Sdk);
            var sdks = existingSdks
                .Concat(sdksToAdd)
                .Distinct();
            project.Xml.Sdk = string.Join($"{Constant.ListSeparator} ", sdks);
        }

        private static IEnumerable<string> ParseSdkList(string sdkList)
            => sdkList == string.Empty
                ? new string[0]
                : sdkList.Split(Constant.ListSeparator).Select(s => s.Trim());
    }
}