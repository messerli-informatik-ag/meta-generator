using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Microsoft.Build.Construction;
using MsBuildProject = Microsoft.Build.Evaluation.Project;

namespace Messerli.FileManipulator.Project
{
    public static class ProjectExtension
    {
        public static ProjectItemGroupElement GetItemGroupWithItemOfTypeOrCreateNew(this MsBuildProject project, string itemType)
            => project
                .GetFirstItemGroupWithItemOfType(itemType)
                .GetOrElse(() => project.CreateItemGroup());

        public static Option<ProjectItemGroupElement> GetFirstItemGroupWithItemOfType(this MsBuildProject project, string itemType)
            => GetItemGroups(project)
                .FirstOrNone(itemGroupElement => itemGroupElement.Items.Any(item => item.ItemType == itemType));

        private static IEnumerable<ProjectItemGroupElement> GetItemGroups(MsBuildProject project)
            => project
                .Xml
                .Items
                .Select(item => item.Parent)
                .OfType<ProjectItemGroupElement>()
                .Distinct();

        private static ProjectItemGroupElement CreateItemGroup(this MsBuildProject project)
            => project.Xml.AddItemGroup();
    }
}
