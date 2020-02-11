namespace Messerli.NativeProjectsPlugin
{
    internal interface IProjectInformation
    {
        string ProjectGuid { get; }

        string ProjectPath();

        string ProjectPath(string fileName);

        string SourcePath(string fileName);

        string HeaderPath(string fileName);

        string PropertyPath(string fileName);
    }
}