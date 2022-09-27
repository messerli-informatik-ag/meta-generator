using Funcky.Monads;

namespace Messerli.FileManipulatorAbstractions;

public sealed class NugetPackageSource
{
    public NugetPackageSource(
        string name,
        string source,
        Option<string> username = default,
        Option<string> password = default,
        Option<string> validAuthenticationTypes = default,
        Option<bool> storePasswordInClearText = default)
    {
        Name = name;
        Source = source;
        Username = username;
        Password = password;
        ValidAuthenticationTypes = validAuthenticationTypes;
        StorePasswordInClearText = storePasswordInClearText.GetOrElse(false);
    }

    public string Name { get; }

    public string Source { get; }

    public Option<string> Username { get; }

    public Option<string> Password { get; }

    public Option<string> ValidAuthenticationTypes { get; }

    public bool StorePasswordInClearText { get; }
}
