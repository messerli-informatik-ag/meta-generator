using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Construction;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project
{
    public static class ProjectExtension
    {
        public static ProjectItemGroupElement GetItemGroupWithItemOfTypeOrCreateNew(this MsBuildProject project, string itemType)
            => project.GetFirstItemGroupWithItemOfType(itemType) ?? project.CreateItemGroup();

        private static ProjectItemGroupElement? GetFirstItemGroupWithItemOfType(this MsBuildProject project, string itemType)
            => GetItemGroups(project)
                .Where(itemGroupElement => itemGroupElement.Items.Any(item => item.ItemType == itemType))?
                .FirstOrDefault();

        private static IEnumerable<ProjectItemGroupElement> GetItemGroups(MsBuildProject project)
            => project
                .Items
                .Where(i => !i.IsImported)
                .Select(item => item.Xml.Parent)
                .OfType<ProjectItemGroupElement>()
                .Distinct();

        private static ProjectItemGroupElement CreateItemGroup(this MsBuildProject project)
            => project.Xml.AddItemGroup();
    }
}