using System.Collections;
using System.Collections.Generic;

namespace Messerli.FileManipulatorAbstractions.Project
{
    public abstract class DependencyAssets
    {
        public sealed class All : DependencyAssets
        {
        }

        public sealed class None : DependencyAssets
        {
        }

        public sealed class List : DependencyAssets, IEnumerable<DependencyAssetName>
        {
            private readonly IEnumerable<DependencyAssetName> _values;

            public List(IEnumerable<DependencyAssetName> values)
            {
                _values = values;
            }

            public List(params DependencyAssetName[] assets)
                : this(assets as IEnumerable<DependencyAssetName>)
            {
            }

            public IEnumerator<DependencyAssetName> GetEnumerator() => _values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}