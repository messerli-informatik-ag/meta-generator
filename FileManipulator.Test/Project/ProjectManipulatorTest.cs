using System.IO;
using System.Threading.Tasks;
using Messerli.FileManipulator.Project;
using Messerli.FileManipulator.Project.MsBuild;
using Messerli.FileManipulatorAbstractions.Project;
using Messerli.Test.Utility;
using Xunit;
using static System.Environment;
using IProjectManipulator = Messerli.FileManipulatorAbstractions.Project.IProjectManipulator;

namespace Messerli.FileManipulator.Test.Project;

public sealed class ProjectManipulatorTest
{
    private const string ProjectFileName = "Foo.csproj";

    private const string PackagesPropsFileName = "Packages.props";

    private const string CentralPackageVersionsSdk = "Microsoft.Build.CentralPackageVersions/2.0.52";

    private static readonly string EmptyProject =
        $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
        $"</Project>{NewLine}";

    private static readonly string EmptyProjectsWithCentralPackageVersionsSdk =
        $"<Project Sdk=\"{CentralPackageVersionsSdk}\">{NewLine}" +
        $"</Project>{NewLine}";

    private static readonly string ProjectWithFooDependency
        = $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
          $"    <ItemGroup>{NewLine}" +
          $"        <PackageReference Include=\"Foo\" Version=\"1.0.0\" />{NewLine}" +
          $"    </ItemGroup>{NewLine}" +
          $"</Project>{NewLine}";

    private static readonly string EmptyPackageProps =
        $"<Project>{NewLine}" +
        $"</Project>{NewLine}";

    private static readonly ProjectModification EmptyModification =
        new ProjectModificationBuilder().Build();

    private static readonly ProjectModification ModificationAddingBarPackageReference =
        new ProjectModificationBuilder()
            .AddPackageReference(new PackageReferenceBuilder()
                .Name("Bar")
                .Version("2.0.0")
                .Build())
            .Build();

    [Theory]
    [MemberData(nameof(GetModifications))]
    public async Task AppliesModificationsSuccessfully(string expectedProject, string existingProject, ProjectModification modification)
    {
        using var testEnvironment = new TestEnvironmentProvider();

        var projectFilePath = Path.Combine(testEnvironment.RootDirectory, ProjectFileName);
        await File.WriteAllTextAsync(projectFilePath, existingProject);

        var projectManipulator = CreateProjectManipulator();
        await projectManipulator.ManipulateProject(projectFilePath, modification);

        Assert.Equal(expectedProject, await File.ReadAllTextAsync(projectFilePath));
    }

    public static TheoryData<string, string, ProjectModification> GetModifications()
        => new()
        {
            {
                $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Include=\"Bar\" Version=\"2.0.0\" />{NewLine}" +
                $"        <PackageReference Include=\"Foo\" Version=\"1.0.0\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                ProjectWithFooDependency,
                ModificationAddingBarPackageReference
            },
            {
                $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                $"  <ItemGroup>{NewLine}" +
                $"    <PackageReference Include=\"Bar\" Version=\"2.0.0\" />{NewLine}" +
                $"  </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                EmptyProject,
                ModificationAddingBarPackageReference
            },
            {
                $"<Project Sdk=\"{CentralPackageVersionsSdk}\">{NewLine}" +
                $"  <ItemGroup>{NewLine}" +
                $"    <PackageReference Include=\"Bar\" Version=\"2.0.0\" />{NewLine}" +
                $"  </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                EmptyProjectsWithCentralPackageVersionsSdk,
                ModificationAddingBarPackageReference
            },
            {
                ProjectWithFooDependency,
                ProjectWithFooDependency,
                EmptyModification
            },
            {
                EmptyProject,
                EmptyProject,
                EmptyModification
            },
            {
                $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                $"  <ItemGroup>{NewLine}" +
                $"    <PackageReference Include=\"Bar\" Version=\"2.0.0\" PrivateAssets=\"all\" IncludeAssets=\"runtime;analyzers;build\" ExcludeAssets=\"none\" />{NewLine}" +
                $"  </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                EmptyProject,
                new ProjectModificationBuilder()
                    .AddPackageReference(new PackageReferenceBuilder()
                        .Name("Bar")
                        .Version("2.0.0")
                        .PrivateAssets(new DependencyAssets.All())
                        .IncludeAssets(new DependencyAssets.List(DependencyAssetName.Runtime, DependencyAssetName.Analyzers, DependencyAssetName.Build))
                        .ExcludeAssets(new DependencyAssets.None())
                        .Build())
                    .Build()
            },
            {
                $"<Project Sdk=\"Microsoft.NET.Sdk; Microsoft.Build.NoTargets\">{NewLine}" +
                $"</Project>{NewLine}",
                EmptyProject,
                new ProjectModificationBuilder()
                    .AddSdk("Microsoft.Build.NoTargets")
                    .Build()
            },
            {
                $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project>{NewLine}" +
                $"</Project>{NewLine}",
                new ProjectModificationBuilder()
                    .AddSdk("Microsoft.NET.Sdk")
                    .Build()
            },
            {
                $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                $"</Project>{NewLine}",
                EmptyProject,
                new ProjectModificationBuilder()
                    .AddSdk("Microsoft.NET.Sdk")
                    .Build()
            },
            {
                ProjectWithFooDependency,
                ProjectWithFooDependency,
                new ProjectModificationBuilder()
                    .AddPackageReference(new PackageReferenceBuilder()
                        .Name("Foo")
                        .Version("1.0.0")
                        .Build())
                    .Build()
            },
        };

    [Fact]
    public async Task ThrowsWhenProjectFileDoesNotExist()
    {
        using var testEnvironment = new TestEnvironmentProvider();

        var projectFilePath = Path.Combine(testEnvironment.RootDirectory, ProjectFileName);

        var projectManipulator = CreateProjectManipulator();

        var exception = await Assert.ThrowsAsync<ProjectManipulationException>(async () =>
        {
            await projectManipulator.ManipulateProject(projectFilePath, new ProjectModificationBuilder().Build());
        });
        Assert.True(exception.InnerException is FileNotFoundException);
    }

    [Theory]
    [MemberData(nameof(GetModificationsForProjectWithCentralPackageVersionsSdk))]
    public async Task AddsPackageReferenceToProjectAndPackagesProps(
        string expectedProject,
        string expectedPackagesProps,
        string existingProject,
        string existingPackagesProps,
        ProjectModification modification)
    {
        using var testEnvironment = new TestEnvironmentProvider();

        var projectFilePath = Path.Combine(testEnvironment.RootDirectory, ProjectFileName);
        var packagesPropsFilePath = Path.Combine(testEnvironment.RootDirectory, PackagesPropsFileName);

        await File.WriteAllTextAsync(projectFilePath, existingProject);
        await File.WriteAllTextAsync(packagesPropsFilePath, existingPackagesProps);

        var projectManipulator = CreateProjectManipulator();
        await projectManipulator.ManipulateProject(projectFilePath, modification);

        Assert.Equal(expectedProject, await File.ReadAllTextAsync(projectFilePath));
        Assert.Equal(expectedPackagesProps, await File.ReadAllTextAsync(packagesPropsFilePath));
    }

    public static TheoryData<string, string, string, string, ProjectModification> GetModificationsForProjectWithCentralPackageVersionsSdk()
        => new()
        {
            {
                $"<Project Sdk=\"Microsoft.NET.Sdk; {CentralPackageVersionsSdk}\">{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Include=\"Bar\" />{NewLine}" +
                $"        <PackageReference Include=\"Foo\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project>{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Update=\"Foo\" Version=\"1.0.0\" />{NewLine}" +
                $"        <PackageReference Update=\"Bar\" Version=\"2.0.0\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project Sdk=\"Microsoft.NET.Sdk; {CentralPackageVersionsSdk}\">{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Include=\"Foo\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project>{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Update=\"Foo\" Version=\"1.0.0\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                ModificationAddingBarPackageReference
            },
            {
                $"<Project Sdk=\"Microsoft.NET.Sdk; {CentralPackageVersionsSdk}\">{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Include=\"Messerli.CodeStyle\" PrivateAssets=\"all\" />{NewLine}" +
                $"        <PackageReference Include=\"Messerli.IO\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project>{NewLine}" +
                $"  <ItemGroup>{NewLine}" +
                $"    <PackageReference Update=\"Messerli.IO\" Version=\"0.1.0\" />{NewLine}" +
                $"  </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project Sdk=\"Microsoft.NET.Sdk; {CentralPackageVersionsSdk}\">{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Include=\"Messerli.CodeStyle\" PrivateAssets=\"all\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project>{NewLine}" +
                $"</Project>{NewLine}",
                new ProjectModificationBuilder()
                    .AddPackageReference(
                        new PackageReferenceBuilder()
                            .Name("Messerli.IO")
                            .Version("0.1.0")
                            .Build())
                    .Build()
            },
            {
                $"<Project Sdk=\"{CentralPackageVersionsSdk}\">{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Include=\"Messerli.IO\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project>{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Update=\"Messerli.IO\" Version=\"0.1.0\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project Sdk=\"{CentralPackageVersionsSdk}\">{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Include=\"Messerli.IO\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project>{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Update=\"Messerli.IO\" Version=\"0.1.0\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                new ProjectModificationBuilder()
                    .AddPackageReference(
                        new PackageReferenceBuilder()
                            .Name("Messerli.IO")
                            .Version("0.1.0")
                            .Build())
                    .Build()
            },
            {
                $"<Project Sdk=\"{CentralPackageVersionsSdk}\">{NewLine}" +
                $"  <ItemGroup>{NewLine}" +
                $"    <PackageReference Include=\"Messerli.IO\" />{NewLine}" +
                $"  </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                $"<Project>{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Update=\"Messerli.IO\" Version=\"0.1.0\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                EmptyProjectsWithCentralPackageVersionsSdk,
                $"<Project>{NewLine}" +
                $"    <ItemGroup>{NewLine}" +
                $"        <PackageReference Update=\"Messerli.IO\" Version=\"0.1.0\" />{NewLine}" +
                $"    </ItemGroup>{NewLine}" +
                $"</Project>{NewLine}",
                new ProjectModificationBuilder()
                    .AddPackageReference(
                        new PackageReferenceBuilder()
                            .Name("Messerli.IO")
                            .Version("0.1.0")
                            .Build())
                    .Build()
            },
        };

    [Fact]
    public async Task ThrowsWhenAddingPackageVersionWithConflictingVersion()
    {
        using var testEnvironment = new TestEnvironmentProvider();

        const string packageName = "Foo";
        const string packageVersionOne = "1.0.0";
        const string packageVersionTwo = "1.0.1";

        var projectFilePath = Path.Combine(testEnvironment.RootDirectory, ProjectFileName);
        await File.WriteAllTextAsync(projectFilePath, EmptyProjectsWithCentralPackageVersionsSdk);

        var projectManipulator = CreateProjectManipulator();

        await projectManipulator.ManipulateProject(
            projectFilePath,
            new ProjectModificationBuilder()
                .AddPackageReference(new PackageReferenceBuilder()
                    .Name(packageName)
                    .Version(packageVersionOne)
                    .Build())
                .Build());

        await Assert.ThrowsAsync<ProjectManipulationException>(async () =>
        {
            await projectManipulator.ManipulateProject(
                projectFilePath,
                new ProjectModificationBuilder()
                    .AddPackageReference(new PackageReferenceBuilder()
                        .Name(packageName)
                        .Version(packageVersionTwo)
                        .Build())
                    .Build());
        });
    }

    [Fact]
    public async Task ThrowsWhenAddingPackageVersionWithConflictingVersionToProjectUsingCentralPackageVersionsSdk()
    {
        using var testEnvironment = new TestEnvironmentProvider();

        const string packageName = "Foo";
        const string packageVersionOne = "1.0.0";
        const string packageVersionTwo = "1.0.1";

        var projectFilePath = Path.Combine(testEnvironment.RootDirectory, ProjectFileName);
        var packagesPropsFilePath = Path.Combine(testEnvironment.RootDirectory, PackagesPropsFileName);

        await File.WriteAllTextAsync(projectFilePath, EmptyProjectsWithCentralPackageVersionsSdk);
        await File.WriteAllTextAsync(packagesPropsFilePath, EmptyPackageProps);

        var projectManipulator = CreateProjectManipulator();

        await projectManipulator.ManipulateProject(
            projectFilePath,
            new ProjectModificationBuilder()
                .AddPackageReference(new PackageReferenceBuilder()
                    .Name(packageName)
                    .Version(packageVersionOne)
                    .Build())
                .Build());

        await Assert.ThrowsAsync<ProjectManipulationException>(async () =>
        {
            await projectManipulator.ManipulateProject(
                projectFilePath,
                new ProjectModificationBuilder()
                    .AddPackageReference(new PackageReferenceBuilder()
                        .Name(packageName)
                        .Version(packageVersionTwo)
                        .Build())
                    .Build());
        });
    }

    private static IProjectManipulator CreateProjectManipulator()
        => new MsBuildProjectManipulatorFacade(
            new MicrosoftBuildAssemblyLoader(),
            new ProjectManipulator(
                new ProjectSdkManipulator(),
                new CentralPackageVersionsManipulator(),
                new PackageReferenceConflictChecker()));
}
