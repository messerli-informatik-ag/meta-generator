namespace Messerli.MetaGenerator
{
    public class GlobalOptions : IGlobalOptions
    {
        public GlobalOptions(bool verbose)
        {
            Verbose = verbose;
        }

        public bool Verbose { get; }
    }
}
