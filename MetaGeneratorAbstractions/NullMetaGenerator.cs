using System;

namespace Messerli.MetaGeneratorAbstractions;

public class NullMetaGenerator : IMetaGenerator
{
    private static readonly Lazy<NullMetaGenerator> LazyInstance =
        new(() => new NullMetaGenerator());

    public string Description { get; } = string.Empty;

    public string Name { get; } = string.Empty;

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
