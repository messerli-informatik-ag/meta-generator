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

namespace Messerli.MesserliOneRepositoryPlugin
{
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
            return new SelectionValue
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

        private bool IsEol(DotNetSdk sdk)
        {
            return sdk.EndOfLife != null
                   && sdk.EndOfLife != "TBA"
                   && ToDate(sdk.EndOfLife)
                       .Match(false, eol => eol < DateTime.Now);
        }

        private Option<DateTime> ToDate(string sdkEndOfLife)
        {
            var dateParts = sdkEndOfLife.Split('-');

            if (dateParts.Length == 3)
            {
                return from year in dateParts[0].TryParseInt()
                       from month in dateParts[1].TryParseInt()
                       from day in dateParts[2].TryParseInt()
                       select new DateTime(year, month, day);
            }

            return Option<DateTime>.None();
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

            return (List<DotNetSdk>)_jsonSerializer
                .ReadObject(stream);
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

        private HashSet<string> ParseSdks(string sdkOutput)
        {
            return sdkOutput
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Where(s => s.Trim().Length != 0)
                .Select(s => s.Substring(0, s.IndexOf(' ')))
                .ToHashSet();
        }
    }
}