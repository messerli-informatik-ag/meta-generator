using System;
using System.IO;
using System.Threading.Tasks;
using Messerli.FileManipulatorAbstractions;
using Messerli.FileSystem;
using Messerli.Test.Utility;
using Xunit;

namespace Messerli.FileManipulator.Test;

public sealed class GlobalJsonManipulatorTest
{
    private const string FileName = "global.json";
    private const string MsBuildSdkNugetPackageId = "Microsoft.Build.NoTargets";
    private const string MsBuildSdkVersion = "1.0.0";
    private static readonly string NewLine = Environment.NewLine;

    private static readonly GlobalJsonModification ModificationThatAddsAnMsBuildSdk =
        new GlobalJsonModificationBuilder()
            .AddMsBuildSdk(new MsBuildSdk(MsBuildSdkNugetPackageId, MsBuildSdkVersion))
            .Build();

    [Theory]
    [MemberData(nameof(GetModificationsToExistingFile))]
    public async Task AppliesModificationsToExistingConfigFile(string expectedConfig, string existingConfig, GlobalJsonModification modification)
    {
        using var testEnvironment = new TestEnvironmentProvider();
        var filePath = Path.Combine(testEnvironment.RootDirectory, FileName);

        await File.WriteAllTextAsync(filePath, existingConfig);

        var globalJsonManipulator = CreateGlobalJsonManipulator();
        await globalJsonManipulator.ModifyGlobalJson(filePath, modification);

        Assert.Equal(expectedConfig, await File.ReadAllTextAsync(filePath));
    }

    [Theory]
    [MemberData(nameof(GetModificationsToNewFile))]
    public async Task AppliesModificationsToNewConfigFile(string expectedConfig, GlobalJsonModification modification)
    {
        using var testEnvironment = new TestEnvironmentProvider();
        var filePath = Path.Combine(testEnvironment.RootDirectory, FileName);

        var globalJsonManipulator = CreateGlobalJsonManipulator();
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

        var globalJsonManipulator = CreateGlobalJsonManipulator();
        await globalJsonManipulator.ModifyGlobalJson(filePath, new GlobalJsonModificationBuilder()
            .AddMsBuildSdk(new MsBuildSdk(nugetPackageId, versionOne))
            .Build());
        var exception = await Assert.ThrowsAsync<GlobalJsonManipulationException>(async () =>
        {
            await globalJsonManipulator.ModifyGlobalJson(filePath, new GlobalJsonModificationBuilder()
                .AddMsBuildSdk(new MsBuildSdk(nugetPackageId, versionTwo))
                .Build());
        });
        Assert.True(exception.InnerException is ConflictingMsBuildSdkException);
    }

    [Fact]
    public async Task ThrowsWhenMsbuildSdksKeyHasIncorrectType()
    {
        var existingConfig = $"{{{NewLine}" +
                             $"    \"msbuild-sdks\": [1, 2, 3]" +
                             $"}}";

        using var testEnvironment = new TestEnvironmentProvider();
        var filePath = Path.Combine(testEnvironment.RootDirectory, FileName);

        await File.WriteAllTextAsync(filePath, existingConfig);

        var globalJsonManipulator = CreateGlobalJsonManipulator();
        var exception = await Assert.ThrowsAsync<GlobalJsonManipulationException>(async () =>
        {
            await globalJsonManipulator.ModifyGlobalJson(filePath, ModificationThatAddsAnMsBuildSdk);
        });
        Assert.True(exception.InnerException is MalformedGlobalJsonException);
    }

    public static TheoryData<string, string, GlobalJsonModification> GetModificationsToExistingFile()
        => new()
        {
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
                ModificationThatAddsAnMsBuildSdk
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
                ModificationThatAddsAnMsBuildSdk
            },
        };

    public static TheoryData<string, GlobalJsonModification> GetModificationsToNewFile()
        => new()
        {
            {
                $"{{{NewLine}" +
                $"    \"msbuild-sdks\": {{{NewLine}" +
                $"        \"{MsBuildSdkNugetPackageId}\": \"{MsBuildSdkVersion}\"{NewLine}" +
                $"    }}{NewLine}" +
                $"}}",
                ModificationThatAddsAnMsBuildSdk
            },
            {
                $"{{{NewLine}" +
                $"    \"msbuild-sdks\": {{{NewLine}" +
                $"        \"A.Build\": \"1.0.0\",{NewLine}" +
                $"        \"B.Build\": \"1.5.0\"{NewLine}" +
                $"    }}{NewLine}" +
                $"}}",
                new GlobalJsonModificationBuilder()
                    .AddMsBuildSdk(new MsBuildSdk("A.Build", "1.0.0"))
                    .AddMsBuildSdk(new MsBuildSdk("B.Build", "1.5.0"))
                    .Build()
            },
        };

    private static IGlobalJsonManipulator CreateGlobalJsonManipulator()
        => new GlobalJsonManipulator(new FileOpeningBuilder());
}
