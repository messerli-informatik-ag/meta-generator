using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using apophis.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.GlobalSection
{
    internal class ProjectConfigurationPlatformsSection : IGlobalSection
    {
        public void Parse(TokenWalker tokenWalker, Solution solution)
        {
            while (tokenWalker.NextIs<EndGlobalSectionToken>() == false)
            {
                tokenWalker.ConsumeAllWhiteSpace();
                var project = ConsumeProject(tokenWalker, solution);
                tokenWalker.Consume<DotToken>();
                var platformAndConfig = tokenWalker.ConsumeBareString();
                tokenWalker.ConsumeAllWhiteSpace();
                tokenWalker.Consume<AssignToken>();
                var configValue = tokenWalker.ConsumeBareString();
                tokenWalker.ConsumeAllWhiteSpace();

                SetProjectConfiguration(project, ExtractPlatform(solution, platformAndConfig), ExtractConfig(platformAndConfig), configValue);
            }
        }

        public void Serialize(Solution solution, StringBuilder result)
        {
            foreach (var project in solution.Projects)
            {
                foreach (var platform in solution.Platforms.Where(p => project.Configuration.ContainsKey(p)))
                {
                    foreach (var configuration in project.Configuration[platform])
                    {
                        result.AppendLine($"\t\t{project.ProjectGuid.SolutionFormat()}.{platform.Config}.{configuration.Config} = {configuration.Value}");
                    }
                }
            }
        }

        public bool Exists(Solution solution)
        {
            return solution.Projects.Any() && solution.Platforms.Any();
        }

        private string ExtractConfig(string platformAndOption)
        {
            return string.Join(".", platformAndOption.Split(".").Skip(1));
        }

        private PlatformConfiguration ExtractPlatform(Solution solution, string platformAndOption)
        {
            var platformKey = platformAndOption.Split(".").First();

            return solution.Platforms.First(p => p.Config == platformKey);
        }

        private static Project ConsumeProject(TokenWalker tokenWalker, Solution solution)
        {
            var projectGuid = tokenWalker.ConsumeGuid();

            return solution.Projects.First(p => p.ProjectGuid == projectGuid);
        }

        private void SetProjectConfiguration(Project project, PlatformConfiguration platformConfiguration, string configName, string configValue)
        {
            if (project.Configuration.ContainsKey(platformConfiguration) == false)
            {
                project.Configuration[platformConfiguration] = new List<PlatformConfiguration>();
            }

            project.Configuration[platformConfiguration].Add(new PlatformConfiguration(configName, configValue));
        }
    }
}
