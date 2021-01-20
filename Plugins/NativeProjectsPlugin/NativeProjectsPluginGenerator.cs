using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funcky;
using Funcky.Extensions;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Messerli.ToolLoaderAbstractions;

namespace Messerli.NativeProjectsPlugin
{
    internal class NativeProjectsPluginGenerator : IMetaGenerator
    {
        private const string VariableDeclarations = "Messerli.NativeProjectsPlugin.templates.VariableDeclarations.json";
        private const string ProjectFileTemplate = "Messerli.NativeProjectsPlugin.templates.ProjectName.vcxproj.template";
        private const string ProjectFileFilterTemplate = "";
        private const string SignListTemplate = "Messerli.NativeProjectsPlugin.templates.SignList.template";
        private const string ExampleSourceTemplate = "";
        private const string ExampleHeaderTemplate = "";
        private const string StdafxHeaderTemplate = "";
        private const string StdafxSourceTemplate = "";
        private const string DebugPropertyTemplate = "";
        private const string ReleasePropertyTemplate = "";
        private const string UsePropertyTemplate = "";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IFileManipulator _fileManipulator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IVariableProvider _variableProvider;
        private readonly IProjectInformation _projectInformation;
        private readonly ITfsPaths _tfsPaths;
        private readonly ITools _tools;

        public NativeProjectsPluginGenerator(
            IConsoleWriter consoleWriter,
            IFileGenerator fileGenerator,
            IFileManipulator fileManipulator,
            IUserInputProvider userInputProvider,
            IVariableProvider variableProvider,
            IProjectInformation projectInformation,
            ITfsPaths tfsPaths,
            ITools tools)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _fileManipulator = fileManipulator;
            _userInputProvider = userInputProvider;
            _variableProvider = variableProvider;
            _projectInformation = projectInformation;
            _tfsPaths = tfsPaths;
            _tools = tools;
        }

        public string Description => "This plugin creates native C++ projects for the All Projects solution.";

        public string Name => "rmiprod-native";

        public void Register()
        {
            _tools.RegisterTool("tfs", "tf.exe");

            _userInputProvider.RegisterVariablesFromTemplate(VariableDeclarations);

            _userInputProvider[Variable.Branch].VariableSelectionValues.AddRange(_tfsPaths.Branches());

            _variableProvider.RegisterValue(Variable.ProjectGuid, _projectInformation.ProjectGuid);
        }

        public void Prepare()
        {
            var tfs = _tools.GetTool("tfs");
            var branchPath = _userInputProvider.Value(Variable.Branch);

            tfs.Execute(new[] { "checkout", AllSolutionPath() }, ".");

            foreach (var signFileList in SignFileLists())
            {
                if (File.Exists(signFileList))
                {
                    tfs.Execute(new[] { "checkout", signFileList }, Path.Combine(branchPath, "Build", "AdditionalBuildSteps", "Sign"));
                }
                else
                {
                    throw new Exception($"Expected file does not exist: {signFileList}");
                }
            }
        }

        public void Generate()
        {
            var projectName = _userInputProvider.Value(Variable.ProjectName);

            _consoleWriter.WriteLine($"Creating the C++ Project '{projectName}' for the All Projects.sln.");

            var tasks = new List<Task>
            {
                _fileGenerator.FromTemplate(ProjectFileTemplate, _projectInformation.ProjectPath($"{projectName}.vcxproj"), Encoding.UTF8),
                _fileGenerator.FromTemplate(ProjectFileFilterTemplate, _projectInformation.ProjectPath($"{projectName}.vcxproj.filters"), Encoding.UTF8),

                _fileGenerator.FromTemplate(ExampleSourceTemplate, _projectInformation.SourcePath($"{projectName}.cpp"), Encoding.UTF8),
                _fileGenerator.FromTemplate(ExampleHeaderTemplate, _projectInformation.HeaderPath($"{projectName}.h"), Encoding.UTF8),

                _fileGenerator.FromTemplate(StdafxHeaderTemplate, _projectInformation.SourcePath("stdafx.h"), Encoding.UTF8),
                _fileGenerator.FromTemplate(StdafxSourceTemplate, _projectInformation.SourcePath("stdafx.cpp"), Encoding.UTF8),

                _fileGenerator.FromTemplate(DebugPropertyTemplate, _projectInformation.PropertyPath($"{projectName}Debug.props"), Encoding.UTF8),
                _fileGenerator.FromTemplate(ReleasePropertyTemplate, _projectInformation.PropertyPath($"{projectName}Release.props"), Encoding.UTF8),
                _fileGenerator.FromTemplate(UsePropertyTemplate, _projectInformation.PropertyPath($"Use{projectName}.props"), Encoding.UTF8),

                _fileManipulator.AddProjectsToSolution(GetSolutionInfoBuilder().Build(), Sequence.FromNullable(GetProjectInfoBuilder().Build())),
            };

            tasks.AddRange(SignFileLists().Select(s => _fileManipulator.AppendTemplate(SignListTemplate, s)));

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
            var tfs = _tools.GetTool("tfs");

            tfs.Execute(new[] { "add", _projectInformation.ProjectPath(), "/recursive" }, _projectInformation.ProjectPath());
        }

        private SolutionInfo.Builder GetSolutionInfoBuilder()
        {
            return new SolutionInfo.Builder()
                .WithPath(AllSolutionPath());
        }

        private ProjectInfo.Builder GetProjectInfoBuilder()
        {
            var projectName = _userInputProvider.Value(Variable.ProjectName);

            return new ProjectInfo.Builder()
                            .WithName(_userInputProvider.Value(Variable.ProjectName))
                            .WithPath(_projectInformation.ProjectPath($"{projectName}.vcxproj"))
                            .WithGuid(Guid.Parse(_userInputProvider.Value(Variable.ProjectGuid)));
        }

        private string AllSolutionPath()
        {
            var branchPath = _userInputProvider.Value(Variable.Branch);

            return Directory
                .GetFiles(Path.Combine(branchPath, "AllProjects"))
                .First(IsAllSolution);
        }

        private static bool IsAllSolution(string x)
        {
            return Path.GetFileName(x).StartsWith("All Projects") && Path.GetFileName(x).EndsWith(".sln");
        }

        private IEnumerable<string> SignFileLists()
        {
            yield return "FileList_Win32.txt";
            yield return "FileList_x64.txt";
        }
    }
}
