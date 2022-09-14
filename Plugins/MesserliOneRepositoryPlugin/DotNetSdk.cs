using System.Runtime.Serialization;

namespace Messerli.MesserliOneRepositoryPlugin;

/// <summary>
/// The current .NET SDKs can be found here:
/// https://dotnet.microsoft.com/download/dotnet-core .
/// </summary>
[DataContract]
public class DotNetSdk
{
    [DataMember]
    public string? Released { get; protected set; }

    [DataMember]
    public string? EndOfLife { get; protected set; }

    [DataMember]
    public string? DotNetVersion { get; protected set; }

    [DataMember]
    public string? ReleaseVersion { get; protected set; }

    [DataMember]
    public string? SdkVersion { get; protected set; }

    [DataMember]
    public bool IsLts { get; protected set; }
}
