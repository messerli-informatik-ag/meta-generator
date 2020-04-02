using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions.Project;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project
{
    public sealed class ProjectManipulator : IProjectManipulator
    {
        private const string PackageReferenceTypeTag = "PackageReference";
        private const string VersionTag = "Version";

        private readonly IMicrosoftBuildAssemblyLoader _microsoftBuildAssemblyLoader;

        public ProjectManipulator(IMicrosoftBuildAssemblyLoader microsoftBuildAssemblyLoader)
        {
            _microsoftBuildAssemblyLoader = microsoftBuildAssemblyLoader;
        }

        public Task ManipulateProject(string projectFilePath, ProjectModification modification)
        {
            _microsoftBuildAssemblyLoader.LoadMicrosoftBuildIfNecessary();
            ManipulateProjectInternal(projectFilePath, modification);
            return Task.CompletedTask;
        }

        private static void ManipulateProjectInternal(string projectFilePath, ProjectModification modification)
        {
            using var projectCollection = new ProjectCollection();
            var project = OpenProject(projectFilePath, projectCollection);
            AddPackageReferencesToProject(project, modification.PackageReferencesToAdd);
            project.Save();
        }

        private static MsBuildProject OpenProject(string projectFilePath, ProjectCollection projectCollection)
        {
            var projectRootElement = ProjectRootElement.Open(projectFilePath, projectCollection, preserveFormatting: true);
            return new MsBuildProject(projectRootElement);
        }

        private static void AddPackageReferencesToProject(MsBuildProject project, IEnumerable<PackageReference> packageReferences)
        {
            if (packageReferences.Any())
            {
                var itemGroup = project.GetItemGroupWithItemOfTypeOrCreateNew(PackageReferenceTypeTag);

                foreach (var packageReference in packageReferences)
                {
                    AddPackageReference(itemGroup, packageReference);
                }
            }
        }

        private static void AddPackageReference(ProjectItemGroupElement itemGroup, PackageReference packageReference)
        {
            var item = itemGroup.AddItem(PackageReferenceTypeTag, packageReference.Name);
            item.AddMetadataAsAttribute(VersionTag, packageReference.Version);
        }
    }
}