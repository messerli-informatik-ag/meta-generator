namespace Messerli.ProjectAbstractions
{
    public interface IFileGenerator
    {
        void FromTemplate(string templatename, string destinationPath);
    }
}
