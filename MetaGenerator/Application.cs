﻿using System.CommandLine;
using Messerli.MetaGeneratorAbstractions;

namespace Messerli.MetaGenerator;

internal class Application : IApplication
{
    private readonly IRootCommandBuilder _rootCommandBuilder;

    public Application(IRootCommandBuilder rootCommandBuilder)
    {
        _rootCommandBuilder = rootCommandBuilder;
    }

    public int Run(string[] args)
        => _rootCommandBuilder
            .Build()
            .Invoke(args);
}
