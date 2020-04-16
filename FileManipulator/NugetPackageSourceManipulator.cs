using System;
using System.IO;
using Messerli.FileManipulatorAbstractions;
using NuGet.Commands;
using NuGet.Common;
using NuGet.Configuration;

namespace Messerli.FileManipulator
{
    public delegate ILogger GetLogger();

    public sealed class NugetPackageSourceManipulator : INugetPackageSourceManipulator
    {
        private const string MinimalNugetConfigContent =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<configuration>\n" +
            "</configuration>\n";

        private readonly Func<ILogger> _getLogger;

        public NugetPackageSourceManipulator(GetLogger getLogger)
        {
            _getLogger = () => getLogger();
        }

        public void Add(string configFile, NugetPackageSource packageSource)
        {
            if (!ExistsPackageSource(configFile, packageSource))
            {
                AddNew(configFile, packageSource);
            }
            else
            {
                Update(configFile, packageSource);
            }
        }

        public void Update(string configFile, NugetPackageSource packageSource)
        {
            var updateSourceArgs = new UpdateSourceArgs
            {
                Configfile = configFile,

                Name = packageSource.Name,
                Password = packageSource.Password,
                Source = packageSource.Source,
                Username = packageSource.Username,
                ValidAuthenticationTypes = packageSource.ValidAuthenticationTypes,
                StorePasswordInClearText = packageSource.StorePasswordInClearText,
            };

            UpdateSourceRunner.Run(updateSourceArgs, _getLogger);
        }

        public void Remove(string configFile, string name)
        {
            var removeSourceArgs = new RemoveSourceArgs
            {
                Configfile = configFile,
                Name = name,
            };

            RemoveSourceRunner.Run(removeSourceArgs, _getLogger);
        }

        public void Enable(string configFile, string name)
        {
            var enableSourceArgs = new EnableSourceArgs
            {
                Configfile = configFile,
                Name = name,
            };

            EnableSourceRunner.Run(enableSourceArgs, _getLogger);
        }

        public void Disable(string configFile, string name)
        {
            var disableSourceArgs = new DisableSourceArgs
            {
                Configfile = configFile,
                Name = name,
            };

            DisableSourceRunner.Run(disableSourceArgs, _getLogger);
        }

        private static void CreateMinimalConfigIfNotExistOrIsEmpty(string configFile)
        {
            if (!File.Exists(configFile) || string.IsNullOrWhiteSpace(File.ReadAllText(configFile)))
            {
                File.WriteAllText(configFile, MinimalNugetConfigContent);
            }
        }

        private void AddNew(string configFile, NugetPackageSource packageSource)
        {
            var addSourceArgs = new AddSourceArgs
            {
                Configfile = configFile,

                Name = packageSource.Name,
                Password = packageSource.Password,
                Source = packageSource.Source,
                Username = packageSource.Username,
                ValidAuthenticationTypes = packageSource.ValidAuthenticationTypes,
                StorePasswordInClearText = packageSource.StorePasswordInClearText,
            };

            CreateMinimalConfigIfNotExistOrIsEmpty(configFile);
            AddSourceRunner.Run(addSourceArgs, _getLogger);
        }

        private static bool ExistsPackageSource(string configFile, NugetPackageSource packageSource)
        {
            var configFileFullPath = Path.GetFullPath(configFile);
            var configDirectory = Path.GetDirectoryName(configFileFullPath);
            var configFileName = Path.GetFileName(configFileFullPath);

            var settings = Settings.LoadSpecificSettings(configDirectory, configFileName);
            var existsPackageSource = new PackageSourceProvider(settings).GetPackageSourceBySource(packageSource.Source);

            return existsPackageSource is { };
        }
    }
}
