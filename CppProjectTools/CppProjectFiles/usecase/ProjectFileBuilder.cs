using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace CppProjectFiles
{
    public class ProjectFileBuilder
    {
        private readonly string _rootNodenameDefault = "Project";
        private readonly string _rootNamespaceDefault = "http://schemas.microsoft.com/developer/msbuild/2003";
        private string _rootNodename = string.Empty;
        private string _rootNamespace = string.Empty;

        // todo: those needs to be included in the right spot
        // private readonly string _searchPhrase = "__declspec(dll";
        // private string _workingNode = "//Project/ItemDefinitionGroup/ClCompile/PreprocessorDefinitions";
        // private string _workingNode = "//Project/ImportGroup";
        private string _workingNode = string.Empty;
        private string _filepath = string.Empty;

        public ProjectFileBuilder SetRootNodename(string name)
        {
            _rootNodename = name;
            return this;
        }

        public ProjectFileBuilder SetRootNamespace(string name)
        {
            _rootNamespace = name;
            return this;
        }

        public ProjectFileBuilder SetRootWorkingNode(string name)
        {
            _workingNode = name;
            return this;
        }

        public ProjectFileBuilder SetXmlFilepath(string name)
        {
            _filepath = name;
            return this;
        }

        public IXmlFile? Build()
        {
            if (!CanBuild())
            {
                return null;
            }

            var classParams = new ProjectFileClassParams
            {
                XmlLoader = new StringFileLoader(_filepath),
                XmlSaver = new StringFileSaver(_filepath),
                XmlWalker = new XmlFileWalker(new XmlFileWalkerClassParams
                {
                    RootNamespace = (_rootNamespace.Length == 0) ? _rootNamespaceDefault : _rootNamespace,
                    RootNodename = (_rootNodename.Length == 0) ? _rootNodenameDefault : _rootNodename,
                    WorkingNode = _workingNode,
                }),
                XmlDataAdder = new XmlDataAdder(),
            };

            return new ProjectFile(classParams);
        }

        private bool CanBuild()
        {
            bool canBuild = true;

            return canBuild;
        }
    }
}
