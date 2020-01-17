using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Messerli.MetaGeneratorAbstractions.UserInput;

namespace Messerli.MetaGeneratorAbstractions.Json
{
    [DataContract]
    public class Variable
    {
        [DataMember]
        public string? Name { get; set; }

        [DataMember]
        public string? Question { get; set; }

        [DataMember]
        public string? Description { get; set; }

        [DataMember]
        public string? Type { get; set; }

        [DataMember]
        public List<SelectionValue>? SelectionValues { get; set; }

        public VariableType GetVariableType() => Enum.Parse<VariableType>(Type);
    }
}
