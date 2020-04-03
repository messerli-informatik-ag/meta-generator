using System;
using System.IO;
using Messerli.FileManipulatorAbstractions;
using Messerli.Test.Utility;
using Moq;
using NuGet.Common;
using NuGet.Configuration;
using Xunit;

namespace Messerli.FileManipulator.Test
{
    public sealed class NugetPackageSourcesTest
    {
        private const string ExamplePackageName = "My Nuget Server";
        private static readonly string NewLine = $"{Environment.NewLine}";

        private static readonly string MinimalNugetConfig =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + $"{NewLine}" +
            "<configuration>" + $"{NewLine}" +
            "</configuration>" + $"{NewLine}";

        private static readonly string NugetConfigWithExample =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + $"{NewLine}" +
            "<configuration>" + $"{NewLine}" +
            "  <packageSources>" + $"{NewLine}" +
            "    <add key=\"My Nuget Server\" value=\"https://nuget.example.ch/v3/index.json\" />" + $"{NewLine}" +
            "  </packageSources>" + $"{NewLine}" +
            "</configuration>" + $"{NewLine}";

        private static readonly string NugetConfigWithPasswordExample =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + $"{NewLine}" +
            "<configuration>" + $"{NewLine}" +
            "  <packageSources>" + $"{NewLine}" +
            "    <add key=\"My Nuget Server\" value=\"https://nuget.example.ch/v3/index.json\" />" + $"{NewLine}" +
            "  </packageSources>" + $"{NewLine}" +
            "</configuration>" + $"{NewLine}";

        private static readonly string NugetConfigWithClearAndNugetOrg =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + $"{NewLine}" +
            "<configuration>" + $"{NewLine}" +
            "  <fallbackPackageFolders>" + $"{NewLine}" +
            "    <clear />" + $"{NewLine}" +
            "  </fallbackPackageFolders>" + $"{NewLine}" +
            "  <packageSources>" + $"{NewLine}" +
            "    <clear />" + $"{NewLine}" +
            "    <add key=\"Nuget\" value=\"https://api.nuget.org/v3/index.json\" protocolVersion=\"3\" />" + $"{NewLine}" +
            "  </packageSources>" + $"{NewLine}" +
            "</configuration>" + $"{NewLine}";

        private static readonly string NugetConfigWithClearAndNugetOrgAndExample =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + $"{NewLine}" +
            "<configuration>" + $"{NewLine}" +
            "  <fallbackPackageFolders>" + $"{NewLine}" +
            "    <clear />" + $"{NewLine}" +
            "  </fallbackPackageFolders>" + $"{NewLine}" +
            "  <packageSources>" + $"{NewLine}" +
            "    <clear />" + $"{NewLine}" +
            "    <add key=\"Nuget\" value=\"https://api.nuget.org/v3/index.json\" protocolVersion=\"3\" />" + $"{NewLine}" +
            "    <add key=\"My Nuget Server\" value=\"https://nuget.example.ch/v3/index.json\" />" + $"{NewLine}" +
            "  </packageSources>" + $"{NewLine}" +
            "</configuration>" + $"{NewLine}";

        private static readonly NugetPackageSource ExamplePackageSource =
            new NugetPackageSource(ExamplePackageName, "https://nuget.example.ch/v3/index.json");

        private static readonly NugetPackageSource PasswordExamplePackageSource =
            new NugetPackageSource(ExamplePackageName, "https://nuget.example.ch/v3/index.json", "Username", "Password");

        [Theory]
        [MemberData(nameof(AddsPackageSourceData))]
        public void AddsPackageSource(string configFileContent, string expectedOutput, NugetPackageSource packageSource)
            => TestPackageSourceCall(
                configFileContent,
                expectedOutput,
                (packageSources, configFile) => packageSources.Add(configFile, packageSource));

        public static TheoryData<string, string, NugetPackageSource> AddsPackageSourceData()
            => new TheoryData<string, string, NugetPackageSource>
            {
                { MinimalNugetConfig, NugetConfigWithExample, ExamplePackageSource },
                { string.Empty, NugetConfigWithExample, ExamplePackageSource },
                { NugetConfigWithClearAndNugetOrg, NugetConfigWithClearAndNugetOrgAndExample, ExamplePackageSource },
            };

        [Theory]
        [MemberData(nameof(UpdatesPackageSourceData))]
        public void UpdatesPackageSource(string configFileContent, string expectedOutput, NugetPackageSource packageSource)
            => TestPackageSourceCall(
                configFileContent,
                expectedOutput,
                (packageSources, configFile) => packageSources.Update(configFile, packageSource));

        public static TheoryData<string, string, NugetPackageSource> UpdatesPackageSourceData()
            => new TheoryData<string, string, NugetPackageSource>
            {
                { NugetConfigWithExample, NugetConfigWithPasswordExample, PasswordExamplePackageSource },
            };

        [Theory]
        [MemberData(nameof(RemovesPackageSourceData))]
        public void RemovesPackageSource(string configFileContent, string expectedOutput, string packageName)
            => TestPackageSourceCall(
                configFileContent,
                expectedOutput,
                (packageSources, configFile) => packageSources.Remove(configFile, packageName));

        public static TheoryData<string, string, string> RemovesPackageSourceData()
            => new TheoryData<string, string, string>
            {
                { NugetConfigWithClearAndNugetOrgAndExample, NugetConfigWithClearAndNugetOrg, ExamplePackageName },
            };

        private void TestPackageSourceCall(
            string fileContent,
            string expectedFileContent,
            Action<INugetPackageSources, string> packageSourceAction)
        {
            using var testEnvironment = new TestEnvironmentProvider();
            CreateNugetTestFile(testEnvironment, fileContent);

            var packageSources = CreateNugetPackageSources();
            packageSourceAction(packageSources, GetNugetFilePath(testEnvironment));

            Assert.Equal(expectedFileContent, ReadNugetTestFile(testEnvironment));
        }

        private static INugetPackageSources CreateNugetPackageSources()
            => new NugetPackageSources(() => new Mock<ILogger>().Object);

        private string GetNugetFilePath(TestEnvironmentProvider testEnvironmentProvider)
            => Path.Combine(testEnvironmentProvider.RootDirectory, "nuget.config");

        private void CreateNugetTestFile(TestEnvironmentProvider testEnvironmentProvider, string fileContent)
            => File.WriteAllText(GetNugetFilePath(testEnvironmentProvider), fileContent);

        private string ReadNugetTestFile(TestEnvironmentProvider testEnvironmentProvider)
            => File.ReadAllText(GetNugetFilePath(testEnvironmentProvider));
    }
}