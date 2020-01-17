using System.Runtime.Serialization;

namespace Messerli.ProjectAbstractions.Json
{
    [DataContract]
    public class SelectionValue
    {
        [DataMember]
        public string? Value { get; set; }

        [DataMember]
        public string? Description { get; set; }
    }
}