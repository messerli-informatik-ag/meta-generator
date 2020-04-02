using System;
using System.IO;
using System.Threading.Tasks;
using Messerli.FileManipulator.Project;
using Messerli.FileManipulatorAbstractions.Project;
using Messerli.FileManipulatorAbstractions.Project.AssetList;
using Messerli.Test.Utility;
using Xunit;
using static System.Environment;

namespace Messerli.FileManipulator.Test.Project
{
    public sealed class ProjectManipulatorTest
    {
        private const string ProjectFileName = "Foo.csproj";

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
        [MemberData(nameof(GetPackageReferenceModificationData))]
        public async Task AddsPackageReferenceToExistingItemGroup(string expectedProject, string existingProject, ProjectModification modification)
        {
            using var testEnvironment = new TestEnvironmentProvider();

            var projectFilePath = Path.Combine(testEnvironment.RootDirectory, ProjectFileName);
            await File.WriteAllTextAsync(projectFilePath, existingProject);

            var projectManipulator = new ProjectManipulator(new MicrosoftBuildAssemblyLoader());
            await projectManipulator.ManipulateProject(projectFilePath, modification);

            Assert.Equal(expectedProject, await File.ReadAllTextAsync(projectFilePath));
        }

        public static TheoryData<string, string, ProjectModification> GetPackageReferenceModificationData()
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
                            .PrivateAssets(new All())
                            .IncludeAssets(new List(AssetName.Runtime, AssetName.Analyzers, AssetName.Build))
                            .ExcludeAssets(new None())
                            .Build())
                        .Build()
                },
            };

        [Fact]
        public void AddsPackageReferenceToProjectAndPackagesProps()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void AddsSdkToProjectWithExistingSdk()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void AddsSdkToProjectWithNoExistingSdk()
        {
            throw new NotImplementedException();
        }
    }
}