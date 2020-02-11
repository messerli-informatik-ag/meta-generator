namespace Messerli.VsSolution.Model
{
    public class SolutionProperty
    {
        public SolutionProperty(string property, string propertyValue)
        {
            Property = property;
            PropertyValue = propertyValue;
        }

        public string Property { get; }

        public string PropertyValue { get; }
    }
}