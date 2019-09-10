namespace Messerli.TfsClient
{
    public interface IClient
    {
        void AddToWorkspace(string path);

        void CheckOutFile(string path);

        string GetPath();
    }
}