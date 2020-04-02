using Microsoft.Build.Locator;

namespace Messerli.FileManipulator
{
    public sealed class MicrosoftBuildAssemblyLoader : IMicrosoftBuildAssemblyLoader
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