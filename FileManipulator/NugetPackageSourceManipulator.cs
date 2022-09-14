using System;
using System.IO;
using Messerli.FileManipulatorAbstractions;
using NuGet.Commands;
using NuGet.Common;

namespace Messerli.FileManipulator;

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
        var addSourceArgs = new AddSourceArgs
        {
            Configfile = configFile,

            Name = packageSource.Name,
            Password = packageSource.Password.GetOrElse(string.Empty),
            Source = packageSource.Source,
            Username = packageSource.Username.GetOrElse(string.Empty),
            ValidAuthenticationTypes = packageSource.ValidAuthenticationTypes.GetOrElse(string.Empty),
            StorePasswordInClearText = packageSource.StorePasswordInClearText,
        };

        CreateMinimalConfigIfNotExistOrIsEmpty(configFile);
        AddSourceRunner.Run(addSourceArgs, _getLogger);
    }

    public void Update(string configFile, NugetPackageSource packageSource)
    {
        var updateSourceArgs = new UpdateSourceArgs
        {
            Configfile = configFile,

            Name = packageSource.Name,
            Password = packageSource.Password.GetOrElse(string.Empty),
            Source = packageSource.Source,
            Username = packageSource.Username.GetOrElse(string.Empty),
            ValidAuthenticationTypes = packageSource.ValidAuthenticationTypes.GetOrElse(string.Empty),
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
}
