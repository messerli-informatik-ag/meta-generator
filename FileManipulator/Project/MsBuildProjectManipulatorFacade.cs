using System;
using System.IO;
using System.Threading.Tasks;
using Messerli.FileManipulator.Project.MsBuild;
using Messerli.FileManipulatorAbstractions.Project;
using Microsoft.Build.Exceptions;
using IProjectManipulator = Messerli.FileManipulatorAbstractions.Project.IProjectManipulator;

namespace Messerli.FileManipulator.Project
{
    internal sealed class MsBuildProjectManipulatorFacade : IProjectManipulator
    {
        private readonly IMicrosoftBuildAssemblyLoader _microsoftBuildAssemblyLoader;

        private readonly MsBuild.IProjectManipulator _projectManipulator;

        public MsBuildProjectManipulatorFacade(
            IMicrosoftBuildAssemblyLoader microsoftBuildAssemblyLoader,
            MsBuild.IProjectManipulator projectManipulator)
        {
            _microsoftBuildAssemblyLoader = microsoftBuildAssemblyLoader;
            _projectManipulator = projectManipulator;
        }

        public Task ManipulateProject(string projectFilePath, ProjectModification modification)
        {
            _microsoftBuildAssemblyLoader.LoadMicrosoftBuildIfNecessary();
            WrapExceptions(projectFilePath, () => _projectManipulator.ManipulateProject(projectFilePath, modification));
            return Task.CompletedTask;
        }

        private static void WrapExceptions(string projectFilePath, Action action)
        {
            try
            {
                action();
            }
            catch (InvalidProjectFileException exception) when (exception.InnerException is FileNotFoundException)
            {
                throw new ProjectManipulationException(exception.InnerException, projectFilePath);
            }
            catch (Exception exception)
            {
                throw new ProjectManipulationException(exception, projectFilePath);
            }
        }
    }
}