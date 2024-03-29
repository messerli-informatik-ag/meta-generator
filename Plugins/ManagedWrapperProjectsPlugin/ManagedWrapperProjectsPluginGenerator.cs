﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Funcky;
using Funcky.Monads;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;
using Messerli.ToolLoaderAbstractions;
using Messerli.VsSolution.Model;
using Soltys.ChangeCase;

namespace Messerli.ManagedWrapperProjectsPlugin;

internal class ManagedWrapperProjectsPluginGenerator : IMetaGenerator
{
    private const string TfsProjectRoot = "TfsProjectRoot";
    private const string HeaderDirectory = "Include";
    private const string SourceDirectory = "Source";
    private const string PropertyDirectory = "Property";

    private readonly IConsoleWriter _consoleWriter;
    private readonly IFileGenerator _fileGenerator;
    private readonly IFileManipulator _fileManipulator;
    private readonly IUserInputProvider _userInputProvider;
    private readonly IVariableProvider _variableProvider;
    private readonly IPathProvider _pathProvider;
    private readonly ITools _tools;
    private Option<Guid> _projectGuid;

    public ManagedWrapperProjectsPluginGenerator(
        IConsoleWriter consoleWriter,
        IFileGenerator fileGenerator,
        IFileManipulator fileManipulator,
        IUserInputProvider userInputProvider,
        IVariableProvider variableProvider,
        IPathProvider pathProvider,
        ITools tools)
    {
        _consoleWriter = consoleWriter;
        _fileGenerator = fileGenerator;
        _fileManipulator = fileManipulator;
        _userInputProvider = userInputProvider;
        _variableProvider = variableProvider;
        _pathProvider = pathProvider;
        _tools = tools;
    }

    public string Description => "This plugin creates managed C++/clr projects for the All Projects solution to access managed code from C++.";

    public string Name => "rmiprod-managed-wrapper";

    public void Register()
    {
        _tools.RegisterTool("tfs", "tf.exe", _pathProvider.GetVisualStudioToolDirectory());

        _userInputProvider.RegisterVariablesFromTemplate(Template.VariableDeclarations);

        _userInputProvider[Variable.Branch].VariableSelectionValues.AddRange(_pathProvider.GetBranches());

        _variableProvider.RegisterValue(TfsProjectRoot, _pathProvider.GetTfsProjectRoot());

        _projectGuid = Guid.NewGuid();
        _variableProvider.RegisterValue(Variable.ProjectGuid, _projectGuid.ToString()!);
        _variableProvider.RegisterValue(Variable.PropertyFilterGuid, CreateGuid());
        _variableProvider.RegisterValue(Variable.SourceFilterGuid, CreateGuid());
        _variableProvider.RegisterValue(Variable.ResourceFilterGuid, CreateGuid());
        _variableProvider.RegisterValue(Variable.IncludeFilterGuid, CreateGuid());
    }

    public void Prepare()
    {
        _variableProvider.RegisterValue(Variable.ProjectNameConstantCase, _userInputProvider.Value(Variable.ProjectName).ConstantCase());

        var tfs = _tools.GetTool("tfs");

        tfs.Execute(new[] { "checkout", "All Projects (Main).sln" }, _pathProvider.GetSolutionDirectory());

        foreach (var signFileList in SignFileLists())
        {
            if (File.Exists(signFileList))
            {
                tfs.Execute(new[] { "checkout", signFileList }, _pathProvider.GetBuildStepSignDirectory());
            }
            else
            {
                throw new Exception($"Expected file does not exist: {signFileList}");
            }
        }
    }

    public void Generate()
    {
        _consoleWriter.WriteLine("Creating a C++/clr project for RmiProd");
        var projectName = _userInputProvider.Value(Variable.ProjectName);

        var tasks = new List<Task>
        {
            _fileGenerator.FromTemplate(Template.ProjectFile, Path.Combine(_pathProvider.GetProjectPath(), $"{projectName}.vcxproj"), Encoding.UTF8),
            _fileGenerator.FromTemplate(Template.ProjectFilter, Path.Combine(_pathProvider.GetProjectPath(), $"{projectName}.vcxproj.filter"), Encoding.UTF8),
            _fileGenerator.FromTemplate(Template.SampleSource, Path.Combine(_pathProvider.GetProjectPath(), SourceDirectory, projectName, $"{projectName}.cpp"), Encoding.UTF8),
            _fileGenerator.FromTemplate(Template.SampleHeader, Path.Combine(_pathProvider.GetProjectPath(), HeaderDirectory, projectName, $"{projectName}.h"), Encoding.UTF8),
            _fileGenerator.FromTemplate(Template.StdAfxSource, Path.Combine(_pathProvider.GetProjectPath(), SourceDirectory, "stdafx.cpp"), Encoding.UTF8),
            _fileGenerator.FromTemplate(Template.StdAfxHeader, Path.Combine(_pathProvider.GetProjectPath(), HeaderDirectory, "stdafx.h"), Encoding.UTF8),
            _fileGenerator.FromTemplate(Template.PackagesConfig, Path.Combine(_pathProvider.GetProjectPath(), "packages.config"), Encoding.UTF8),
            _fileGenerator.FromTemplate(Template.DebugProperties, Path.Combine(_pathProvider.GetProjectPath(), PropertyDirectory, $"{projectName}Debug.props"), Encoding.UTF8),
            _fileGenerator.FromTemplate(Template.ReleaseProperties, Path.Combine(_pathProvider.GetProjectPath(), PropertyDirectory, $"{projectName}Release.props"), Encoding.UTF8),
            _fileGenerator.FromTemplate(Template.UseProperties, Path.Combine(_pathProvider.GetProjectPath(), PropertyDirectory, $"Use{projectName}.props"), Encoding.UTF8),
            _fileGenerator.FromTemplate(Template.VersionInfo, Path.Combine(_pathProvider.GetVersionInfoPath(),  $"{projectName}Version.h"), Encoding.UTF8),

            _fileManipulator.AppendTemplate(Template.FilesToSign, Path.Combine(_pathProvider.GetBuildStepSignDirectory(), "FileList_Win32.txt")),
            _fileManipulator.AppendTemplate(Template.FilesToSign, Path.Combine(_pathProvider.GetBuildStepSignDirectory(), "FileList_x64.txt")),
            _fileManipulator.AddProjectsToSolution(GetSolutionInfoBuilder().Build(), Sequence.FromNullable(GetProjectInfoBuilder().Build())),
        };

        Task.WaitAll(tasks.ToArray());
    }

    public void TearDown()
    {
        var projectName = _userInputProvider.Value(Variable.ProjectName);

        var tfs = _tools.GetTool("tfs");
        tfs.Execute(new[] { "add", _pathProvider.GetProjectPath(), "/recursive" }, _pathProvider.GetProjectPath());
        tfs.Execute(new[] { "add", Path.Combine(_pathProvider.GetVersionInfoPath(), $"{projectName}Version.h") }, _pathProvider.GetVersionInfoPath());
    }

    private SolutionInfo.Builder GetSolutionInfoBuilder()
    {
        return new SolutionInfo.Builder()
            .WithPath(Path.Combine(_pathProvider.GetSolutionDirectory(), "All Projects (Main).sln"));
    }

    private ProjectInfo.Builder GetProjectInfoBuilder()
    {
        var projectName = _userInputProvider.Value(Variable.ProjectName);

        return new ProjectInfo.Builder()
            .WithName(projectName)
            .WithPath(Path.Combine(_pathProvider.GetProjectPath(), $"{projectName}.vcxproj"))
            .WithGuid(_projectGuid)
            .WithType(ProjectType.Identifier.CPlusPlus);
    }

    private static string CreateGuid()
    {
        return Guid.NewGuid().ToString();
    }

    private IEnumerable<string> SignFileLists()
    {
        var branchPath = _userInputProvider.Value(Variable.Branch);

        yield return Path.Combine(branchPath, "Build", "AdditionalBuildSteps", "Sign", "FileList_Win32.txt");
        yield return Path.Combine(branchPath, "Build", "AdditionalBuildSteps", "Sign", "FileList_x64.txt");
    }
}
