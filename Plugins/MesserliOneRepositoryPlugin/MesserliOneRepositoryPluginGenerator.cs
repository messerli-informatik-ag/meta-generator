using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Messerli.ToolLoaderAbstractions;
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
        private const string PublishScript = "Messerli.MesserliOneRepositoryPlugin.templates.publish.ps1.template";

        private const string ProgramSource = "Messerli.MesserliOneRepositoryPlugin.templates.Program.Source.template";
        private const string ApplicationSource = "Messerli.MesserliOneRepositoryPlugin.templates.Application.Source.template";
        private const string ApplicationInterfaceSource = "Messerli.MesserliOneRepositoryPlugin.templates.IApplication.Source.template";
        private const string CompositionRootSource = "Messerli.MesserliOneRepositoryPlugin.templates.CompositionRoot.Source.template";

        private const string RepositoryNameVariable = "RepositoryName";
        private const string BasePath = "BasePath";
        private const string SolutionItems = "SolutionItems";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly ISolutionLoader _solutionLoader;
        private readonly ITools _tools;

        public MesserliOneRepositoryPluginGenerator(IConsoleWriter consoleWriter, IFileGenerator fileGenerator, IUserInputProvider userInputProvider, ISolutionLoader solutionLoader, ITools tools)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
            _solutionLoader = solutionLoader;
            _tools = tools;
        }

        public string Name => "This will generate a git repository with a new .NET Core Solution according to the Messerli One Standards";

        public string ShortName => "messerli-one.repository";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);
            _tools.RegisterTool("dotnet", "dotnet.exe");
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
                _fileGenerator.FromTemplate(PublishScript, Path.Combine(RepositoryPath(), "publish.ps1"), Encoding.UTF8),
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

            // Create an initial commit
            CommitAll(repo, "Initial commit by the MetaGenerator");

            var dotnet = _tools.GetTool("dotnet");

            dotnet.Execute(new[] { "new", "tool-manifest" }, RepositoryPath());
            CommitAll(repo, "dotnet new tool-manifest");

            dotnet.Execute(new[] { "tool", "install", "paket" }, RepositoryPath());
            CommitAll(repo, "dotnet tool install paket");

            dotnet.Execute(new[] { "paket", "install" }, RepositoryPath());
            CommitAll(repo, "dotnet paket install");

            RunBuild();
        }

        private void CommitAll(Repository repository, string message)
        {
            Commands.Stage(repository, "*");

            // Create an initial commit
            repository.Commit(message, Author(), Commiter(repository));
        }

        private void RunBuild()
        {
            var dotnet = _tools.GetTool("dotnet");

            dotnet.Execute(new[] { "build" }, RepositoryPath());
        }

        private static Signature Commiter(Repository repository)
        {
            var config = repository.Config;

            return config.BuildSignature(DateTimeOffset.Now) ?? Author();
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
            var solution = Solution.NewSolution(solutionPath);

            AddSolutionFolder(solution);

            solution.AddProject(RepositoryName(), Path.Combine(RepositoryPath(), RepositoryName(), $"{RepositoryName()}.csproj"), ProjectType.Identifier.CSharpSdk, null);

            await _solutionLoader.Store(solutionPath, solution);
        }

        private void AddSolutionFolder(Solution solution)
        {
            solution.AddProject(SolutionItems, Path.Combine(RepositoryPath(), RepositoryName(), $"{RepositoryName()}.csproj"), ProjectType.Identifier.SolutionFolder, null);
            var project = solution.Projects.First();

            AddSolutionItemToProject(project, ".gitignore");
            AddSolutionItemToProject(project, "paket.dependencies");
        }

        private static void AddSolutionItemToProject(Project project, string item, string? alias = null)
        {
            project.SolutionItems.Add(new SolutionItem(item, alias ?? item));
        }
    }
}