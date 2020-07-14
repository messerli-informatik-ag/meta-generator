using Microsoft.Build.Locator;

namespace Messerli.FileManipulator.Project.MsBuild
{
    internal sealed class MicrosoftBuildAssemblyLoader : IMicrosoftBuildAssemblyLoader
    {
        public void LoadMicrosoftBuildIfNecessary()
        {
            if (MSBuildLocator.CanRegister)
            {
                MSBuildLocator.RegisterDefaults();
            }
        }
    }
}