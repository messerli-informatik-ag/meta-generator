namespace Messerli.FileManipulatorAbstractions
{
    public sealed class NugetPackageSource
    {
        public NugetPackageSource(
            string name,
            string source,
            string? username = null,
            string? password = null,
            string? validAuthenticationTypes = null,
            bool? storePasswordInClearText = null)
        {
            Name = name;
            Source = source;
            Username = username;
            Password = password;
            ValidAuthenticationTypes = validAuthenticationTypes;
            StorePasswordInClearText = storePasswordInClearText ?? false;
        }

        public string Name { get; }

        public string Source { get; }

        public string? Username { get; }

        public string? Password { get; }

        public string? ValidAuthenticationTypes { get; }

        public bool StorePasswordInClearText { get; }
    }
}