using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Messerli.MetaGenerator.PluginManagement
{
    // https://stackoverflow.com/questions/42174699/how-to-download-a-nupkg-package-from-nuget-programmatically-in-net-core
    // https://docs.microsoft.com/en-us/nuget/reference/nuget-client-sdk
    // https://daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-3
    internal class NugetPluginRepository : IPluginRepository
    {
        private const string RepositoryUri = @"C:\Repositories\meta-generator\nupkg";

        private readonly ILogger _logger;

        public NugetPluginRepository(ILogger logger)
        {
            _logger = logger;
        }

        public string Name => $"Local repository @ {RepositoryUri}";

        async Task<IEnumerable<PluginPackage>> IPluginRepository.Plugins()
        {
            CancellationToken cancellationToken = CancellationToken.None;

            // SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            SourceRepository repository = Repository.Factory.GetCoreV3(RepositoryUri);
            PackageSearchResource resource = await repository.GetResourceAsync<PackageSearchResource>(cancellationToken);
            var searchFilter = new SearchFilter(includePrerelease: true);

            IEnumerable<IPackageSearchMetadata> results = await resource.SearchAsync(string.Empty, searchFilter, skip: 0, take: 20, _logger, cancellationToken);

            return results.Select(r => new PluginPackage(r.Identity.Id));
        }
    }
}
