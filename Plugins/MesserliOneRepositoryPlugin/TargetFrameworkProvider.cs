using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization.Json;
using Funcky.Extensions;
using Funcky.Monads;
using Messerli.MetaGeneratorAbstractions;
using Messerli.MetaGeneratorAbstractions.Json;
using Messerli.ToolLoaderAbstractions;
using Pastel;

namespace Messerli.MesserliOneRepositoryPlugin;

public class TargetFrameworkProvider : ITargetFrameworkProvider
{
    private const string DotNetSdkResource = "Messerli.MesserliOneRepositoryPlugin.templates.DotNetSdks.json";

    private readonly DataContractJsonSerializer _jsonSerializer;
    private readonly ITemplateLoader _templateLoader;
    private readonly ITools _tools;
    private IEnumerable<string>? _installedSdks;

    public TargetFrameworkProvider(ITemplateLoader templateLoader, ITools tools)
    {
        _templateLoader = templateLoader;
        _tools = tools;
        _jsonSerializer = new DataContractJsonSerializer(typeof(List<DotNetSdk>));
    }

    public IEnumerable<SelectionValue> GetSelection()
    {
        return GetSdks()
            .Select(ToSelectionValue)
            .Concat(GetInstalledSdks().Where(NotInSdkList).Select(InstalledToSelectionValue));
    }

    private bool NotInSdkList(string sdkVersion) => GetSdks().All(sdk => sdk.SdkVersion != sdkVersion);

    private SelectionValue InstalledToSelectionValue(string sdkVersion)
    {
        var notRecommended = "Installed but not recommended".Pastel(Color.OrangeRed);

        return new SelectionValue
        {
            Description = $"{sdkVersion} ({notRecommended})",
            Value = sdkVersion,
        };
    }

    private SelectionValue ToSelectionValue(DotNetSdk sdk)
    {
        return new()
        {
            Description = $".NET SDK {sdk.SdkVersion} ({Eol(sdk)})[{InstalledSdk(sdk)}]{Lts(sdk)} [Released: {sdk.Released}]",
            Value = sdk.SdkVersion,
        };
    }

    private string Lts(DotNetSdk sdk)
    {
        var lts = "LTS".Pastel(Color.Green);
        return sdk.IsLts
            ? $"({lts})"
            : string.Empty;
    }

    private string Eol(DotNetSdk sdk)
    {
        return IsEol(sdk)
            ? "EOL".Pastel(Color.OrangeRed)
            : "OK".Pastel(Color.Green);
    }

    private static bool IsEol(DotNetSdk sdk)
        => sdk.EndOfLife is not null
           && sdk.EndOfLife is not "TBA"
           && ToDate(sdk.EndOfLife)
               .Match(false, eol => eol < DateTime.Now);

    private static Option<DateTime> ToDate(string sdkEndOfLife)
    {
        var dateParts = sdkEndOfLife.Split('-');

        return dateParts.Length == 3
            ? from year in dateParts[0].ParseInt32OrNone()
            from month in dateParts[1].ParseInt32OrNone()
            from day in dateParts[2].ParseInt32OrNone()
            select new DateTime(year, month, day)
            : Option<DateTime>.None;
    }

    private string InstalledSdk(DotNetSdk sdk)
    {
        var installed = GetInstalledSdks();

        return installed.Contains(sdk.SdkVersion)
            ? "Installed".Pastel(Color.Green)
            : "Not Installed".Pastel(Color.OrangeRed);
    }

    private List<DotNetSdk> GetSdks()
    {
        using var stream = _templateLoader.GetTemplateStream(DotNetSdkResource);

        var list = Option
            .FromNullable(stream)
            .Match(
                none: () => throw new Exception("no stream"),
                some: s => Option.FromNullable((List<DotNetSdk>?)_jsonSerializer.ReadObject(s)));

        return list.GetOrElse(() => throw new Exception("read object failed"));
    }

    private IEnumerable<string> GetInstalledSdks()
    {
        return _installedSdks ??= GetInstalledSdksFromEnvironment();
    }

    private IEnumerable<string> GetInstalledSdksFromEnvironment()
    {
        var dotnet = _tools.GetTool("dotnet");

        dotnet.Execute(new[] { "--list-sdks" }, "C:/");

        return ParseSdks(dotnet.StandardOutput);
    }

    private static HashSet<string> ParseSdks(string sdkOutput)
        => sdkOutput
            .SplitLines()
            .Where(s => s.Trim().Length != 0)
            .Select(s => s.Substring(0, s.IndexOf(' ')))
            .ToHashSet();
}
