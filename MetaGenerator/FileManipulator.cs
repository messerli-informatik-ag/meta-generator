using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Funcky.Extensions;
using Messerli.CommandLineAbstractions;
using Messerli.MetaGeneratorAbstractions;
using Messerli.VsSolution;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace Messerli.MetaGenerator;

internal class FileManipulator : IFileManipulator
{
    private readonly IVariableProvider _variableProvider;
    private readonly ITemplateLoader _templateLoader;
    private readonly StubbleBuilder _stubbleBuilder;
    private readonly ISolutionLoader _solutionLoader;
    private readonly IConsoleWriter _consoleWriter;

    public FileManipulator(
        IVariableProvider variableProvider,
        ITemplateLoader templateLoader,
        StubbleBuilder stubbleBuilder,
        ISolutionLoader solutionLoader,
        IConsoleWriter consoleWriter)
    {
        _variableProvider = variableProvider;
        _templateLoader = templateLoader;
        _stubbleBuilder = stubbleBuilder;
        _solutionLoader = solutionLoader;
        _consoleWriter = consoleWriter;
    }

    public async Task AppendTemplate(string templateName, string filePath)
    {
        _consoleWriter.WriteLine($"Append template '{templateName}' to '{filePath}'");

        if (File.Exists(filePath) == false)
        {
            throw new Exception($"cannot append to file '{filePath}' it does not exist.");
        }

        await using var sw = File.AppendText(filePath);
        await sw.WriteAsync(await OutputFromTemplate(templateName));
    }

    public async Task AddProjectsToSolution(SolutionInfo solutionInfo, IEnumerable<ProjectInfo> projectInfos)
    {
        var solution = await _solutionLoader.Load(solutionInfo.Path);

        projectInfos
            .ForEach(projectInfo => solution.AddProject(projectInfo.Name, projectInfo.Path, projectInfo.Type, projectInfo.Guid));

        solutionInfo.FilterFolder.AndThen(ff
            => projectInfos.ForEach(projectInfo
                => solution.AddNestedProject(ff, projectInfo.Name)));

        await _solutionLoader.Store(solutionInfo.Path, solution);
    }

    private async Task<string> OutputFromTemplate(string templateName)
    {
        var stubble = _stubbleBuilder
            .Configure(StubbleBuilderSettings)
            .Build();

        return await stubble.RenderAsync(_templateLoader.GetTemplate(templateName), _variableProvider.GetVariableValues(), TemplateRenderSettings());
    }

    private static void StubbleBuilderSettings(RendererSettingsBuilder settings)
    {
        settings.SetIgnoreCaseOnKeyLookup(false);
    }

    private static RenderSettings TemplateRenderSettings()
    {
        return new()
        {
            ThrowOnDataMiss = true,
            SkipHtmlEncoding = true,
        };
    }
}
