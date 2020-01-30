using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Messerli.VsSolution;
using Messerli.VsSolution.Model;

namespace Messerli.MesserliOneRepositoryPlugin
{
    public class MesserliOneRepositoryPluginGenerator : IMetaGenerator
    {
        private const string VariableDeclarations = "Messerli.MesserliOneRepositoryPlugin.templates.VariableDeclarations.json";
        private const string DirectoryBuildTargets = "Messerli.MesserliOneRepositoryPlugin.templates.Directory.Build.targets.template";
        private const string PaketDependencies = "Messerli.MesserliOneRepositoryPlugin.templates.paket.dependencies.template";
        private const string PaketReferences = "Messerli.MesserliOneRepositoryPlugin.templates.paket.references.template";
        private const string GitIgnore = "Messerli.MesserliOneRepositoryPlugin.templates.gitignore.template";
        private const string ProjectFile = "Messerli.MesserliOneRepositoryPlugin.templates.Project.csproj.template";

        private const string ProgramSource = "Messerli.MesserliOneRepositoryPlugin.templates.Program.Source.template";
        private const string ApplicationSource = "Messerli.MesserliOneRepositoryPlugin.templates.Application.Source.template";
        private const string ApplicationInterfaceSource = "Messerli.MesserliOneRepositoryPlugin.templates.IApplication.Source.template";
        private const string CompositionRootSource = "Messerli.MesserliOneRepositoryPlugin.templates.CompositionRoot.Source.template";

        private const string RepositoryNameVariable = "RepositoryName";
        private const string BasePath = "BasePath";
        private const string InitialCommitMessage = "Initial commit by the MetaGenerator";
        private const string PaketInstallCommitMessage = "paket install by the MetaGenerator";
        private const string SolutionItems = "SolutionItems";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly ISolutionLoader _solutionLoader;

        public MesserliOneRepositoryPluginGenerator(IConsoleWriter consoleWriter, IFileGenerator fileGenerator, IUserInputProvider userInputProvider, ISolutionLoader solutionLoader)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
            _solutionLoader = solutionLoader;
        }

        public string Name => "This will generate a git repository with a new .NET Core Solution according to the Messerli One Standards";

        public string ShortName => "messerli-one.repository";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);
        }

        public void Prepare()
        {
        }

        public void Generate()
        {
            _consoleWriter.WriteLine($"Creating Repository: {RepositoryName()}");

            var tasks = new List<Task>
            {
                _fileGenerator.FromTemplate(DirectoryBuildTargets, Path.Combine(RepositoryPath(), "Directory.Build.targets"), Encoding.UTF8),
                _fileGenerator.FromTemplate(GitIgnore, Path.Combine(RepositoryPath(), ".gitignore"), Encoding.UTF8),
                _fileGenerator.FromTemplate(ProjectFile, Path.Combine(RepositoryPath(), RepositoryName(), $"{RepositoryName()}.csproj"), Encoding.UTF8),
                _fileGenerator.FromTemplate(PaketDependencies, Path.Combine(RepositoryPath(), "paket.dependencies"), Encoding.UTF8),

                _fileGenerator.FromTemplate(ProgramSource, Path.Combine(RepositoryPath(), RepositoryName(), "Program.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(ApplicationSource, Path.Combine(RepositoryPath(), RepositoryName(), "Application.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(ApplicationInterfaceSource, Path.Combine(RepositoryPath(), RepositoryName(), "IApplication.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(CompositionRootSource, Path.Combine(RepositoryPath(), RepositoryName(), "CompositionRoot.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(PaketReferences, Path.Combine(RepositoryPath(), RepositoryName(), "paket.references"), Encoding.UTF8),

                GenerateSolution(),
            };

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
            Repository.Init(RepositoryPath());

            // Add created files to repository: git add --all <RepositoryPath>
            using var repo = new Repository(RepositoryPath());
            Commands.Stage(repo, "*");

            // Create an initial commit
            repo.Commit(InitialCommitMessage, Author(), Commiter());

            // paket install here...
            if (RunPaketInstall())
            {
                // add generated Paket.restore.target andpaket.lock files to the repository
                Commands.Stage(repo, "*");

                // Commit these changes
                repo.Commit(PaketInstallCommitMessage, Author(), Commiter());
            }
        }

        public bool RunPaketInstall()
        {
            if (ExistsOnPath("paket.exe"))
            {
                var paketInstall = new ProcessStartInfo(GetFullPath("paket.exe"))
                {
                    Arguments = "install",
                    WorkingDirectory = RepositoryPath(),
                };
                using var process = Process.Start(paketInstall);

                process?.WaitForExit();
                return true;
            }

            _consoleWriter.WriteLine("paket.exe is not found in your machines PATH.");
            _consoleWriter.WriteLine("Install paket.exe and execute 'paket install' in the generated directory and commit the two generated files.");

            return false;
        }

        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        public static string? GetFullPath(string fileName)
        {
            return File.Exists(fileName)
                ? Path.GetFullPath(fileName)
                : Environment.GetEnvironmentVariable("PATH")
                    ?.Split(Path.PathSeparator)
                    .Select(path => Path.Combine(path, fileName))
                    .FirstOrDefault(File.Exists);
        }

        private static Signature Commiter()
        {
            return new Signature("Thomas Bruderer", "thomas.bruderer@messerli.ch", DateTime.Now);
        }

        private static Signature Author()
        {
            return new Signature("Meta Generator", "meta-generator@messerli.ch", DateTime.Now);
        }

        private string RepositoryName()
        {
            return _userInputProvider.Value(RepositoryNameVariable);
        }

        private string RepositoryPath()
        {
            return Path.Combine(_userInputProvider.Value(BasePath), RepositoryName());
        }

        private async Task GenerateSolution()
        {
            var solutionPath = Path.Combine(RepositoryPath(), $"{RepositoryName()}.sln");
            var solution = new Solution(solutionPath)
            {
                FormatVersion = 12,
                VisualStudioVersion = new System.Version(16, 0, 29709, 97),
                MinimumVisualStudioVersion = new System.Version(10, 0, 40219, 1),
                Guid = Guid.NewGuid(),
            };

            solution.Platforms.Add(new PlatformConfiguration("Debug|Any CPU", "Debug|Any CPU"));
            solution.Platforms.Add(new PlatformConfiguration("Release|Any CPU", "Release|Any CPU"));

            solution.AddProject(SolutionItems, Path.Combine(RepositoryPath(), RepositoryName(), $"{RepositoryName()}.csproj"), ProjectType.Identifier.SolutionFolder, null);
            var solutionItems = solution.Projects.First();

            solutionItems.SolutionItems.Add(new SolutionItem(".gitignore", ".gitignore"));
            solutionItems.SolutionItems.Add(new SolutionItem("paket.dependencies", "paket.dependencies"));

            solution.AddProject(RepositoryName(), Path.Combine(RepositoryPath(), RepositoryName(), $"{RepositoryName()}.csproj"), ProjectType.Identifier.CSharpSdk, null);

            await _solutionLoader.Store(solutionPath, solution);
        }
    }
}