using System;
using System.IO;
using Messerli.FileManipulatorAbstractions;
using Messerli.Test.Utility;
using Moq;
using NuGet.Common;
using Xunit;

namespace Messerli.FileManipulator.Test
{
    public sealed class NugetPackageSourcesTest
    {
        private const string ExamplePackageName = "My Nuget Server";
        private const string PasswordExamplePackageName = ExamplePackageName;

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

        private static readonly string NugetConfigWithDisabledExample =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + $"{NewLine}" +
            "<configuration>" + $"{NewLine}" +
            "  <packageSources>" + $"{NewLine}" +
            "    <add key=\"My Nuget Server\" value=\"https://nuget.example.ch/v3/index.json\" />" + $"{NewLine}" +
            "  </packageSources>" + $"{NewLine}" +
            "  <disabledPackageSources>" + $"{NewLine}" +
            "    <add key=\"My Nuget Server\" value=\"true\" />" + $"{NewLine}" +
            "  </disabledPackageSources>" + $"{NewLine}" +
            "</configuration>" + $"{NewLine}";

        private static readonly string NugetConfigWithPasswordExample =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + $"{NewLine}" +
            "<configuration>" + $"{NewLine}" +
            "  <packageSources>" + $"{NewLine}" +
            "    <add key=\"My Nuget Server\" value=\"https://nuget.example.ch/v3/index.json\" />" + $"{NewLine}" +
            "  </packageSources>" + $"{NewLine}" +
            "  <packageSourceCredentials>" + $"{NewLine}" +
            "    <My_x0020_Nuget_x0020_Server>" + $"{NewLine}" +
            "        <add key=\"Username\" value=\"Username\" />" + $"{NewLine}" +
            "        <add key=\"ClearTextPassword\" value=\"Password\" />" + $"{NewLine}" +
            "      </My_x0020_Nuget_x0020_Server>" + $"{NewLine}" +
            "  </packageSourceCredentials>" + $"{NewLine}" +
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
            new NugetPackageSource(
                ExamplePackageName,
                "https://nuget.example.ch/v3/index.json",
                "Username",
                "Password",
                null,
                true);

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
                { NugetConfigWithPasswordExample, MinimalNugetConfig, PasswordExamplePackageName },
            };

        [Theory]
        [MemberData(nameof(EnablePackageSourceData))]
        public void EnablePackageSource(string configFileContent, string expectedOutput, string packageName)
            => TestPackageSourceCall(
                configFileContent,
                expectedOutput,
                (packageSources, configFile) => packageSources.Enable(configFile, packageName));

        public static TheoryData<string, string, string> EnablePackageSourceData()
            => new TheoryData<string, string, string>
            {
                { NugetConfigWithDisabledExample, NugetConfigWithExample, ExamplePackageName },
                { NugetConfigWithExample, NugetConfigWithExample, ExamplePackageName },
            };

        [Theory]
        [MemberData(nameof(DisablePackageSourceData))]
        public void DisablePackageSource(string configFileContent, string expectedOutput, string packageName)
            => TestPackageSourceCall(
                configFileContent,
                expectedOutput,
                (packageSources, configFile) => packageSources.Disable(configFile, packageName));

        public static TheoryData<string, string, string> DisablePackageSourceData()
            => new TheoryData<string, string, string>
            {
                { NugetConfigWithExample, NugetConfigWithDisabledExample, ExamplePackageName },
            };

        private void TestPackageSourceCall(
            string fileContent,
            string expectedFileContent,
            Action<INugetPackageSourceManipulator, string> packageSourceAction)
        {
            using var testEnvironment = new TestEnvironmentProvider();
            CreateNugetTestFile(testEnvironment, fileContent);

            var packageSources = CreateNugetPackageSources();
            packageSourceAction(packageSources, GetNugetFilePath(testEnvironment));

            Assert.Equal(expectedFileContent, ReadNugetTestFile(testEnvironment));
        }

        private static INugetPackageSourceManipulator CreateNugetPackageSources(ILogger? logger = null)
            => new NugetPackageSourceManipulator(() => logger ?? new Mock<ILogger>().Object);

        private static string GetNugetFilePath(TestEnvironmentProvider testEnvironmentProvider)
            => Path.Combine(testEnvironmentProvider.RootDirectory, "nuget.config");

        private static void CreateNugetTestFile(TestEnvironmentProvider testEnvironmentProvider, string fileContent)
            => File.WriteAllText(GetNugetFilePath(testEnvironmentProvider), fileContent);

        private static string ReadNugetTestFile(TestEnvironmentProvider testEnvironmentProvider)
            => File.ReadAllText(GetNugetFilePath(testEnvironmentProvider));
    }
}
