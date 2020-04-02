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

        [Theory]
        [MemberData(nameof(GetPackageReferenceModificationData))]
        public async Task AddsPackageReferenceToExistingItemGroup(string expectedProject, string existingProject, ProjectModification modification)
        {
            using var testEnvironment = new TestEnvironmentProvider();

            var projectFilePath = Path.Combine(testEnvironment.RootDirectory, ProjectFileName);
            await File.WriteAllTextAsync(projectFilePath, existingProject);

            var projectManipulator = new ProjectManipulator();
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
                    $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                    $"    <ItemGroup>{NewLine}" +
                    $"        <PackageReference Include=\"Foo\" Version=\"1.0.0\" />{NewLine}" +
                    $"    </ItemGroup>{NewLine}" +
                    $"</Project>{NewLine}",
                    new ProjectModificationBuilder()
                        .AddPackageReference(new PackageReference("Bar", "2.0.0"))
                        .Build()
                },
                {
                    $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                    $"  <ItemGroup>{NewLine}" +
                    $"    <PackageReference Include=\"Bar\" Version=\"2.0.0\" />{NewLine}" +
                    $"  </ItemGroup>{NewLine}" +
                    $"</Project>{NewLine}",
                    $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                    $"</Project>{NewLine}",
                    new ProjectModificationBuilder()
                        .AddPackageReference(new PackageReference("Bar", "2.0.0"))
                        .Build()
                },
                {
                    $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                    $"    <ItemGroup>{NewLine}" +
                    $"        <PackageReference Include=\"Foo\" Version=\"1.0.0\" />{NewLine}" +
                    $"    </ItemGroup>{NewLine}" +
                    $"</Project>{NewLine}",
                    $"<Project Sdk=\"Microsoft.NET.Sdk\">{NewLine}" +
                    $"    <ItemGroup>{NewLine}" +
                    $"        <PackageReference Include=\"Foo\" Version=\"1.0.0\" />{NewLine}" +
                    $"    </ItemGroup>{NewLine}" +
                    $"</Project>{NewLine}",
                    new ProjectModificationBuilder().Build()
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