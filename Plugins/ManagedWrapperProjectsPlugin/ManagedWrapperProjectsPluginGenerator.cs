using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.ManagedWrapperProjectsPlugin
{
    internal class ManagedWrapperProjectsPluginGenerator : IMetaGenerator
    {
        private const string TfsProjectRoot = "TfsProjectRoot";

        private readonly IConsoleWriter _consoleWriter;
        private readonly IFileGenerator _fileGenerator;
        private readonly IFileManipulator _fileManipulator;
        private readonly IUserInputProvider _userInputProvider;
        private readonly IVariableProvider _variableProvider;
        private readonly IPathProvider _pathProvider;

        public ManagedWrapperProjectsPluginGenerator(
            IConsoleWriter consoleWriter,
            IFileGenerator fileGenerator,
            IFileManipulator fileManipulator,
            IUserInputProvider userInputProvider,
            IVariableProvider variableProvider,
            IPathProvider pathProvider)
        {
            _consoleWriter = consoleWriter;
            _fileGenerator = fileGenerator;
            _fileManipulator = fileManipulator;
            _userInputProvider = userInputProvider;
            _variableProvider = variableProvider;
            _pathProvider = pathProvider;
        }

        public string Description => "This plugin creates managed C++/clr projects for the All Projects solution to access managed code from C++.";

        public string Name => "rmiprod-managed-wrapper";

        public void Register()
        {
            _userInputProvider.RegisterVariablesFromTemplate(TemplateName.VariableDeclarations);

            _userInputProvider[Variable.Branch].VariableSelectionValues.AddRange(_pathProvider.GetBranches());

            _variableProvider.RegisterValue(TfsProjectRoot, _pathProvider.GetTfsProjectRoot());

            _variableProvider.RegisterValue(Variable.ProjectGuid, CreateGuid());
            _variableProvider.RegisterValue(Variable.PropertyFilterGuid, CreateGuid());
            _variableProvider.RegisterValue(Variable.SourceFilterGuid, CreateGuid());
            _variableProvider.RegisterValue(Variable.ResourceFilterGuid, CreateGuid());
            _variableProvider.RegisterValue(Variable.IncludeFilterGuid, CreateGuid());
        }

        public void Prepare()
        {
            // for example tfs checkout
        }

        public void Generate()
        {
            _consoleWriter.WriteLine($"Creating a C++/clr project for RmiProd");
            var projectName = _userInputProvider.Value(Variable.ProjectName);

            var tasks = new List<Task>
            {
                _fileGenerator.FromTemplate(TemplateName.ProjectFile, Path.Combine(_pathProvider.GetProjectPath(), $"{projectName}.vcxproj"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.ProjectFilter, Path.Combine(_pathProvider.GetProjectPath(), $"{projectName}.vcxproj.filter"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.SampleSource, Path.Combine(_pathProvider.GetProjectPath(), "Source", projectName, $"{projectName}.cpp"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.SampleHeader, Path.Combine(_pathProvider.GetProjectPath(), "Header", projectName, $"{projectName}.h"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.StdAfxSource, Path.Combine(_pathProvider.GetProjectPath(), "stdafx.cpp"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.StdAfxHeader, Path.Combine(_pathProvider.GetProjectPath(),  "stdafx.h"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.Resource, Path.Combine(_pathProvider.GetProjectPath(), "Resource", $"{projectName}.rc2"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.DllMain, Path.Combine(_pathProvider.GetProjectPath(), "Source", projectName, "dllmain.cpp"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.PackagesConfig, Path.Combine(_pathProvider.GetProjectPath(), "packages.config"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.DebugProperties, Path.Combine(_pathProvider.GetProjectPath(), "Property", $"{projectName}Debug.props"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.ReleaseProperties, Path.Combine(_pathProvider.GetProjectPath(), "Property", $"{projectName}Debug.props"), Encoding.UTF8),
                _fileGenerator.FromTemplate(TemplateName.UseProperties, Path.Combine(_pathProvider.GetProjectPath(), "Property", $"Use{projectName}.props"), Encoding.UTF8),
            };

            Task.WaitAll(tasks.ToArray());
        }

        public void TearDown()
        {
            // for example git add
        }

        private static string CreateGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}