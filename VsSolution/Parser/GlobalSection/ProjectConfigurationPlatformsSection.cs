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
                var config = tokenWalker.ConsumeVariable();

                SetProjectConfiguration(ExtractProject(config.Key, solution), ExtractPlatform(config.Key, solution), ExtractConfig(config.Key), config.Value);
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

        private Project ExtractProject(string configKey, Solution solution)
        {
            var projectGuid = Guid.Parse(configKey.Split(".").First());

            return solution.Projects.First(p => p.ProjectGuid == projectGuid);
        }

        private string ExtractConfig(string configKey)
        {
            return string.Join(".", configKey.Split(".").Skip(2));
        }

        private PlatformConfiguration ExtractPlatform(string configKey, Solution solution)
        {
            var platformKey = configKey.Split(".").Skip(1).First();

            return solution.Platforms.First(p => p.Config == platformKey);
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
