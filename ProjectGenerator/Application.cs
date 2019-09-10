using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;

namespace Messerli.ProjectGenerator
{
    internal class Application : IApplication
    {
        private readonly IConsoleWriter _consoleWriter;
        private readonly IEnumerable<IProjectGenerator> _projectGenerators;
        private readonly RootCommand _rootCommand;

        public Application(IConsoleWriter consoleWriter, IEnumerable<IProjectGenerator> projectGenerators)
        {
            _consoleWriter = consoleWriter;
            _projectGenerators = projectGenerators;

            _rootCommand = SetupRootCommand();
            SetupProjectCommand();
        }

        public int Run(string[] args)
        {
            return _rootCommand.Invoke(args);
        }

        private RootCommand SetupRootCommand()
        {
            var rootCommand = new RootCommand
            {
                Description = "This is the Messerli Project generator",
                Handler = CommandHandler.Create<string>(ExecuteWizard),
            };

            var projectTypeOption = new Option("--project-type", "Give the project-type")
            {
                Argument = new Argument<string?>(defaultValue: () => null),
            };

            rootCommand.AddOption(projectTypeOption);

            return rootCommand;
        }

        private void SetupProjectCommand()
        {
            var projectsCommand = new Command("projects", "List all projects")
            {
                Handler = CommandHandler.Create(ListProjects),
            };

            _rootCommand.AddCommand(projectsCommand);
        }

        private void ListProjects()
        {
            _consoleWriter.WriteLine($"Available project types");

            foreach (var projectGenerator in _projectGenerators)
            {
                _consoleWriter.WriteLine($"* {projectGenerator.ShortName}");
            }
        }

        private void ExecuteWizard(string projectType)
        {
            _consoleWriter.WriteLine("Welcome to the project generator wizard: ");

            var projectTypeGenerator = _projectGenerators
                .FirstOrDefault(generator => generator.ShortName == projectType);

            if (projectTypeGenerator == null)
            {
                _consoleWriter.WriteLine("Bad projectype");
            }
            else
            {
                projectTypeGenerator.Generate();
            }
        }
    }
}