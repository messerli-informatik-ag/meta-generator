using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using Messerli.CommandLineAbstractions;
using Messerli.ProjectAbstractions;

namespace Messerli.ProjectGenerator
{
    internal class Application : IApplication
    {
        private readonly IConsoleWriter _consoleWriter;
        private readonly IEnumerable<IProjectGenerator> _projectGenerators;

        public Application(IConsoleWriter consoleWriter, IEnumerable<IProjectGenerator> projectGenerators)
        {
            _consoleWriter = consoleWriter;
            _projectGenerators = projectGenerators;
        }

        public int Run(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option("--project-type", "Give the project-type")
                {
                    Argument = new Argument<string?>(defaultValue: () => null),
                },
            };

            rootCommand.Description = "This is the Messerli Project generator";
            rootCommand.Handler = CommandHandler.Create<string>(HandleX);

            var projectsCommand = new Command("projects", "List all projects")
            {
                Handler = CommandHandler.Create(ListProjects),
            };

            rootCommand.AddCommand(projectsCommand);

            return rootCommand.Invoke(args);
        }

        private void ListProjects()
        {
            _consoleWriter.WriteLine($"Available project types");

            foreach (var projectGenerator in _projectGenerators)
            {
                _consoleWriter.WriteLine($"* {projectGenerator.ShortName}");
            }
        }

        private static void HandleX(string projectType)
        {
            Console.WriteLine($"The value for --int-option is: {projectType}");
        }
    }
}