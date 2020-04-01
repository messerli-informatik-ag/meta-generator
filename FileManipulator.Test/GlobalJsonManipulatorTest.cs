using System;
using System.IO;
using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions;
using Messerli.FileSystem;
using Messerli.Test.Utility;
using Xunit;

namespace Messerli.FileManipulator.Test
{
    public sealed class GlobalJsonManipulatorTest
    {
        private const string FileName = "global.json";
        private const string MsBuildSdkNugetPackageId = "Microsoft.Build.NoTargets";
        private const string MsBuildSdkVersion = "1.0.0";
        private static readonly string NewLine = Environment.NewLine;

        private static readonly GlobalJsonModification AddMsBuildSdkModification =
            new GlobalJsonModificationBuilder()
                .AddMsBuildSdk(new MsBuildSdk(MsBuildSdkNugetPackageId, MsBuildSdkVersion))
                .Build();

        [Theory]
        [MemberData(nameof(GetModificationData))]
        public async Task AddsMsBuildSdksToConfig(string expectedConfig, string? existingConfig, GlobalJsonModification modification)
        {
            using var testEnvironment = new TestEnvironmentProvider();
            var filePath = Path.Combine(testEnvironment.RootDirectory, FileName);

            if (existingConfig is { })
            {
                await File.WriteAllTextAsync(filePath, existingConfig);
            }

            var globalJsonManipulator = new GlobalJsonManipulator(new FileOpeningBuilder());
            await globalJsonManipulator.ModifyGlobalJson(filePath, modification);

            Assert.Equal(expectedConfig, await File.ReadAllTextAsync(filePath));
        }

        [Fact]
        public async Task ThrowsWhenMsBuildSdkAlreadyExistsWithDifferentVersion()
        {
            const string nugetPackageId = "Foo.Build";
            const string versionOne = "1.0.0";
            const string versionTwo = "2.0.0";

            using var testEnvironment = new TestEnvironmentProvider();
            var filePath = Path.Combine(testEnvironment.RootDirectory, FileName);

            var globalJsonManipulator = new GlobalJsonManipulator(new FileOpeningBuilder());
            await globalJsonManipulator.ModifyGlobalJson(filePath, new GlobalJsonModificationBuilder()
                .AddMsBuildSdk(new MsBuildSdk(nugetPackageId, versionOne))
                .Build());
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await globalJsonManipulator.ModifyGlobalJson(filePath, new GlobalJsonModificationBuilder()
                    .AddMsBuildSdk(new MsBuildSdk(nugetPackageId, versionTwo))
                    .Build());
            });
        }

        public static TheoryData<string, string?, GlobalJsonModification> GetModificationData()
            => new TheoryData<string, string?, GlobalJsonModification>
            {
                {
                    $"{{{NewLine}" +
                    $"    \"msbuild-sdks\": {{{NewLine}" +
                    $"        \"{MsBuildSdkNugetPackageId}\": \"{MsBuildSdkVersion}\"{NewLine}" +
                    $"    }}{NewLine}" +
                    $"}}",
                    null,
                    AddMsBuildSdkModification
                },
                {
                    $"{{{NewLine}" +
                    $"    \"sdk\": {{{NewLine}" +
                    $"        \"version\": \"3.1.200\"{NewLine}" +
                    $"    }},{NewLine}" +
                    $"    \"foo\": \"bar\",{NewLine}" +
                    $"    \"msbuild-sdks\": {{{NewLine}" +
                    $"        \"{MsBuildSdkNugetPackageId}\": \"{MsBuildSdkVersion}\"{NewLine}" +
                    $"    }}{NewLine}" +
                    $"}}",
                    $"{{{NewLine}" +
                    $"    \"sdk\": {{{NewLine}" +
                    $"        \"version\": \"3.1.200\"{NewLine}" +
                    $"    }},{NewLine}" +
                    $"    \"foo\": \"bar\"{NewLine}" +
                    $"}}{NewLine}",
                    AddMsBuildSdkModification
                },
                {
                    $"{{{NewLine}" +
                    $"    \"msbuild-sdks\": {{{NewLine}" +
                    $"        \"{MsBuildSdkNugetPackageId}\": \"{MsBuildSdkVersion}\"{NewLine}" +
                    $"    }}{NewLine}" +
                    $"}}",
                    $"{{{NewLine}" +
                    $"    \"msbuild-sdks\": {{{NewLine}" +
                    $"        \"{MsBuildSdkNugetPackageId}\": \"{MsBuildSdkVersion}\"{NewLine}" +
                    $"    }}{NewLine}" +
                    $"}}{NewLine}",
                    AddMsBuildSdkModification
                },
                {
                    $"{{{NewLine}" +
                    $"    \"msbuild-sdks\": {{{NewLine}" +
                    $"        \"A.Build\": \"1.0.0\",{NewLine}" +
                    $"        \"B.Build\": \"1.5.0\"{NewLine}" +
                    $"    }}{NewLine}" +
                    $"}}",
                    null,
                    new GlobalJsonModificationBuilder()
                        .AddMsBuildSdk(new MsBuildSdk("A.Build", "1.0.0"))
                        .AddMsBuildSdk(new MsBuildSdk("B.Build", "1.5.0"))
                        .Build()
                },
            };
    }
}