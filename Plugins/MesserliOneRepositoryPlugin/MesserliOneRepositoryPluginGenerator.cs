using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funcky.Monads;
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
        private const string RepositoryNameVariable = "RepositoryName";
        private const string BasePath = "BasePath";
        private const string SolutionItems = "SolutionItems";
        private const string TargetFramework = "TargetFramework";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly ISolutionLoader _solutionLoader;
        private readonly ITools _tools;
        private readonly ITargetFrameworkProvider _targetFrameworkProvider;

        public MesserliOneRepositoryPluginGenerator(
            IConsoleWriter consoleWriter,
            IFileGenerator fileGenerator,
            IUserInputProvider userInputProvider,
            ISolutionLoader solutionLoader,
            ITargetFrameworkProvider targetFrameworkProvider,
            ITools tools)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _userInputProvider = userInputProvider;
            _solutionLoader = solutionLoader;
            _targetFrameworkProvider = targetFrameworkProvider;
            _tools = tools;
        }

        public string Description => "This will generate a git repository with a new .NET Core Solution according to the Messerli One Standards";

        public string Name => "messerli-one-repository";

        public void Register()
        {
            _tools.RegisterTool("dotnet", "dotnet.exe");

            _userInputProvider.RegisterVariablesFromTemplate(Template.VariableDeclarations);

            _tools.VerifyTools();
            _userInputProvider[TargetFramework].VariableSelectionValues.AddRange(_targetFrameworkProvider.GetSelection());
        }

        public void Prepare()
        {
        }

        public void Generate()
        {
            _consoleWriter.WriteLine($"Creating Repository: {RepositoryName()}");

            var tasks = new List<Task>
            {
                _fileGenerator.FromTemplate(Template.DirectoryBuildTargets, Path.Combine(RepositoryPath(), "Directory.Build.targets"), Encoding.UTF8),
                _fileGenerator.FromTemplate(Template.GitIgnore, Path.Combine(RepositoryPath(), ".gitignore"), Encoding.UTF8),
                _fileGenerator.FromTemplate(Template.PublishScript, Path.Combine(RepositoryPath(), "publish.ps1"), Encoding.UTF8),
                _fileGenerator.FromTemplate(Template.ProjectFile, Path.Combine(RepositoryPath(), RepositoryName(), $"{RepositoryName()}.csproj"), Encoding.UTF8),

                _fileGenerator.FromTemplate(Template.ProgramSource, Path.Combine(RepositoryPath(), RepositoryName(), "Program.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(Template.ApplicationSource, Path.Combine(RepositoryPath(), RepositoryName(), "Application.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(Template.ApplicationInterfaceSource, Path.Combine(RepositoryPath(), RepositoryName(), "IApplication.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(Template.CompositionRootSource, Path.Combine(RepositoryPath(), RepositoryName(), "CompositionRoot.cs"), Encoding.UTF8),
                _fileGenerator.FromTemplate(Template.GlobalJson, Path.Combine(RepositoryPath(), "global.json"), Encoding.UTF8),
                _fileGenerator.FromTemplate(Template.PackagesProperties, Path.Combine(RepositoryPath(), "Packages.props"), Encoding.UTF8),

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

            dotnet.Execute(new[] { "restore" }, RepositoryPath());
            CommitAll(repo, "dotnet restore");

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
            return new ("Meta Generator", "meta-generator@messerli.ch", DateTime.Now);
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

            solution.AddProject(RepositoryName(), Path.Combine(RepositoryPath(), RepositoryName(), $"{RepositoryName()}.csproj"), ProjectType.Identifier.CSharpSdk);

            await _solutionLoader.Store(solutionPath, solution);
        }

        private void AddSolutionFolder(Solution solution)
        {
            solution.AddProject(SolutionItems, Path.Combine(RepositoryPath(), RepositoryName(), $"{RepositoryName()}.csproj"), ProjectType.Identifier.SolutionFolder);
            var project = solution.Projects.First();

            AddSolutionItemToProject(project, ".gitignore");
            AddSolutionItemToProject(project, "paket.dependencies");
        }

        private static void AddSolutionItemToProject(Project project, string item, Option<string> alias = default)
        {
            project.SolutionItems.Add(new SolutionItem(item, alias.GetOrElse(item)));
        }
    }
}
