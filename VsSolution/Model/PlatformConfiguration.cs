namespace Messerli.VsSolution.Model
{
    public class PlatformConfiguration
    {
        public PlatformConfiguration(string config, string value)
        {
            Config = config;
            Value = value;
        }

        public string Config { get; }

        public string Value { get; }
    }
}