namespace Messerli.TfsClient
{
    public interface ITfsClient
    {
        void AddToWorkspace(string path);

        void CheckOutFile(string path);
    }
}