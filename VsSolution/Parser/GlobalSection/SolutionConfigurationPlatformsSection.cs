﻿using System.Linq;
using System.Text;
using Messerli.Lexer;
using Messerli.VsSolution.Model;
using Messerli.VsSolution.Token;

namespace Messerli.VsSolution.Parser.GlobalSection;

internal class SolutionConfigurationPlatformsSection : IGlobalSection
{
    public void Parse(TokenWalker tokenWalker, Solution solution)
    {
        while (tokenWalker.NextIs<EndGlobalSectionToken>() == false)
        {
            var variable = tokenWalker.ConsumeVariable();
            solution.Platforms.Add(new PlatformConfiguration(variable.Key, variable.Value));
            tokenWalker.ConsumeAllWhiteSpace();
        }
    }

    public void Serialize(Solution solution, StringBuilder result)
    {
        foreach (var platform in solution.Platforms)
        {
            result.AppendLine($"\t\t{platform.Config} = {platform.Value}");
        }
    }

    public bool Exists(Solution solution)
    {
        return solution.Platforms.Any();
    }
}
