using System;

namespace Messerli.VsSolution.Model
{
    public class NestedProject
    {
        public NestedProject(Guid parent, Guid child)
        {
            Parent = parent;
            Child = child;
        }

        public Guid Parent { get; }

        public Guid Child { get; }
    }
}