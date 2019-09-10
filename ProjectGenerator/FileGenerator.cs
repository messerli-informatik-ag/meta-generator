using System.IO;
using System.Reflection;
using Messerli.ProjectAbstractions;

namespace Messerli.ProjectGenerator
{
    public class FileGenerator : IFileGenerator
    {
        private readonly IProjectInformationProvider _projectInformationProvider;

        public FileGenerator(IProjectInformationProvider projectInformationProvider)
        {
            _projectInformationProvider = projectInformationProvider;
        }

        public void FromTemplate(string templateName, string relativePath)
        {
            var path = Path.Combine(_projectInformationProvider.DestinationPath, relativePath);
            CreateMissingDirectories(path);

            File.WriteAllText(path, GetTemplate(templateName, Assembly.GetCallingAssembly()));
        }

        private static string GetTemplate(string templateName, Assembly assembly)
        {
            using (var templateStream = assembly.GetManifestResourceStream(templateName))
            {
                if (templateStream != null)
                {
                    using (StreamReader reader = new StreamReader(templateStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            return string.Empty;
        }

        private void CreateMissingDirectories(string path)
        {
            var folder = Path.GetDirectoryName(path);

            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}
