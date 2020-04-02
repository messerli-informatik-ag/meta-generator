using System;
using System.IO;
using System.Threading.Tasks;
using Messerli.FileManipulator.Project;
using Messerli.FileManipulatorAbstractions.Project;
using Messerli.Test.Utility;
using Xunit;
using static System.Environment;

namespace Messerli.FileManipulator.Test.Project
{
    public sealed class ProjectManipulatorTest
    {
        private const string ProjectFileName = "Foo.csproj";

        private const string PackagesPropsFileName = "Packages.props";

        private static readonly string EmptyProject =
            $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
            $"</Project>{NewLine}";

        private static readonly string ProjectWithFooDependency
            = $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
              $"    <ItemGroup>{NewLine}" +
              $"        <PackageReference Include=\"Foo\" Version=\"1.0.0\" />{NewLine}" +
              $"    </ItemGroup>{NewLine}" +
              $"</Project>{NewLine}";

        [Theory]
        [MemberData(nameof(GetModifications))]
        public async Task AppliesModificationsSuccessfully(string expectedProject, string existingProject, ProjectModification modification)
        {
            using var testEnvironment = new TestEnvironmentProvider();

            var projectFilePath = Path.Combine(testEnvironment.RootDirectory, ProjectFileName);
            await File.WriteAllTextAsync(projectFilePath, existingProject);

            var projectManipulator = new ProjectManipulator(new MicrosoftBuildAssemblyLoader());
            await projectManipulator.ManipulateProject(projectFilePath, modification);

            Assert.Equal(expectedProject, await File.ReadAllTextAsync(projectFilePath));
        }

        public static TheoryData<string, string, ProjectModification> GetModifications()
            => new TheoryData<string, string, ProjectModification>
            {
                {
                    $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                    $"    <ItemGroup>{NewLine}" +
                    $"        <PackageReference Include=\"Bar\" Version=\"2.0.0\" />{NewLine}" +
                    $"        <PackageReference Include=\"Foo\" Version=\"1.0.0\" />{NewLine}" +
                    $"    </ItemGroup>{NewLine}" +
                    $"</Project>{NewLine}",
                    ProjectWithFooDependency,
                    new ProjectModificationBuilder()
                        .AddPackageReference(new PackageReferenceBuilder()
                            .Name("Bar")
                            .Version("2.0.0")
                            .Build())
                        .Build()
                },
                {
                    $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                    $"  <ItemGroup>{NewLine}" +
                    $"    <PackageReference Include=\"Bar\" Version=\"2.0.0\" />{NewLine}" +
                    $"  </ItemGroup>{NewLine}" +
                    $"</Project>{NewLine}",
                    EmptyProject,
                    new ProjectModificationBuilder()
                        .AddPackageReference(new PackageReferenceBuilder()
                            .Name("Bar")
                            .Version("2.0.0")
                            .Build())
                        .Build()
                },
                {
                    ProjectWithFooDependency,
                    ProjectWithFooDependency,
                    new ProjectModificationBuilder().Build()
                },
                {
                    EmptyProject,
                    EmptyProject,
                    new ProjectModificationBuilder().Build()
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
            };

        [Fact]
        public async Task ThrowsWhenProjectFileDoesNotExist()
        {
            using var testEnvironment = new TestEnvironmentProvider();

            var projectFilePath = Path.Combine(testEnvironment.RootDirectory, ProjectFileName);

            var projectManipulator = new ProjectManipulator(new MicrosoftBuildAssemblyLoader());

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

            var projectManipulator = new ProjectManipulator(new MicrosoftBuildAssemblyLoader());
            await projectManipulator.ManipulateProject(projectFilePath, modification);

            Assert.Equal(expectedProject, await File.ReadAllTextAsync(projectFilePath));
            Assert.Equal(expectedPackagesProps, await File.ReadAllTextAsync(packagesPropsFilePath));
        }

        public static TheoryData<string, string, string, string, ProjectModification> GetModificationsForProjectWithCentralPackageVersionsSdk()
            => new TheoryData<string, string, string, string, ProjectModification>
            {
                {
                    $"<Project Sdk=\"Microsoft.NET.Sdk; Microsoft.Build.CentralPackageVersions/2.0.52\">{NewLine}" +
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
                    $"<Project Sdk=\"Microsoft.NET.Sdk; Microsoft.Build.CentralPackageVersions/2.0.52\">{NewLine}" +
                    $"    <ItemGroup>{NewLine}" +
                    $"        <PackageReference Include=\"Foo\" />{NewLine}" +
                    $"    </ItemGroup>{NewLine}" +
                    $"</Project>{NewLine}",
                    $"<Project>{NewLine}" +
                    $"    <ItemGroup>{NewLine}" +
                    $"        <PackageReference Update=\"Foo\" Version=\"1.0.0\" />{NewLine}" +
                    $"    </ItemGroup>{NewLine}" +
                    $"</Project>{NewLine}",
                    new ProjectModificationBuilder()
                        .AddPackageReference(
                            new PackageReferenceBuilder()
                                .Name("Bar")
                                .Version("2.0.0")
                                .Build())
                        .Build()
                },
                {
                    $"<Project Sdk=\"Microsoft.NET.Sdk; Microsoft.Build.CentralPackageVersions/2.0.52\">{NewLine}" +
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
                    $"<Project Sdk=\"Microsoft.NET.Sdk; Microsoft.Build.CentralPackageVersions/2.0.52\">{NewLine}" +
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
            };

        [Fact]
        public void ThrowsWhenPackagesPropsDoesNotExist()
        {
            throw new NotImplementedException();
        }
    }
}