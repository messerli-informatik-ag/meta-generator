using System;

namespace Messerli.ProjectAbstractions
{
    public class NullProjectGenerator : IProjectGenerator
    {
        private static readonly Lazy<NullProjectGenerator> LazyInstance =
            new Lazy<NullProjectGenerator>(() => new NullProjectGenerator());

        public string Name { get; } = string.Empty;

        public string ShortName { get; } = string.Empty;

        public static NullProjectGenerator Instance => LazyInstance.Value;

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