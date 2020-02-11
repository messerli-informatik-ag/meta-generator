namespace Messerli.VsSolution.Model
{
    public class TfsControlProperty
    {
        public TfsControlProperty(string propertyKey, string propertyValue)
        {
            PropertyKey = propertyKey;
            PropertyValue = propertyValue;
        }

        public string PropertyKey { get; }

        public string PropertyValue { get; }
    }
}