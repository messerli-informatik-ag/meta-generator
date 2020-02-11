using System;

namespace Messerli.VsSolution.Model
{
    public class Dependency
    {
        public Dependency(Guid dependent, Guid dependee)
        {
            Dependent = dependent;
            Dependee = dependee;
        }

        public Guid Dependent { get; }

        public Guid Dependee { get; }
    }
}