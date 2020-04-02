using System.Collections.Generic;

namespace Messerli.FileManipulatorAbstractions.Project.AssetList
{
    public sealed class List : IAssetListVariant
    {
        public List(IEnumerable<AssetName> assets)
        {
            Assets = assets;
        }

        public List(params AssetName[] assets)
            : this(assets as IEnumerable<AssetName>)
        {
        }

        public IEnumerable<AssetName> Assets { get; }
    }
}