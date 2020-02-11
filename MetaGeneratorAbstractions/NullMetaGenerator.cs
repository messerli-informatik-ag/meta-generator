using System;

namespace Messerli.MetaGeneratorAbstractions
{
    public class NullMetaGenerator : IMetaGenerator
    {
        private static readonly Lazy<NullMetaGenerator> LazyInstance =
            new Lazy<NullMetaGenerator>(() => new NullMetaGenerator());

        public string Name { get; } = string.Empty;

        public string ShortName { get; } = string.Empty;

        public static NullMetaGenerator Instance => LazyInstance.Value;

        public void Register()
        {
        }

        public void Prepare()
        {
        }

        public void Generate()
        {
        }

        public void TearDown()
        {
        }
    }
}