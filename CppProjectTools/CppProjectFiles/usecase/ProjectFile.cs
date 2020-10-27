using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CppProjectFiles
{
    public sealed class ProjectFile : IXmlFile
    {
        private readonly ProjectFileClassParams _classParams;
        private XmlDocument _xmlData = new XmlDocument();
        private XmlNode _currentNode;

        public ProjectFile(ProjectFileClassParams param)
        {
            _classParams = param;
            _xmlData.LoadXml(_classParams.XmlLoader.GetData());
            _currentNode = (XmlNode)_xmlData.DocumentElement;
        }

        public void SetData(string nodeValue)
        {
            _classParams.XmlDataAdder.SetData(_currentNode, nodeValue);
        }

        public bool Contains(string nodeValue)
        {
            var node = (XmlElement)_currentNode;

            // despite the existance of a ms article this doesn't work:
            // node.InnerText.Contains(nodeValue, StringComparison.OrdinalIgnoreCase);
            return node.InnerText.ToLower().Contains(nodeValue.ToLower());
        }

        public string GetData()
        {
            var nodeData = (XmlElement)_currentNode;
            return nodeData.InnerText;
        }

        public IXmlFile GotoNode(string node)
        {
            _currentNode = _classParams.XmlWalker.VisitNode(_xmlData, node);
            return this;
        }

        public void SetAttribute(string attribute, string value)
        {
            _classParams.XmlDataAdder.SetAttribute(_currentNode, attribute, value);
        }

        public void AddNode(string nodeName)
        {
            _classParams.XmlDataAdder.AddNode(_currentNode, nodeName);
        }

        public void Dispose()
        {
            _classParams.XmlSaver.Save(GetRootNode().OuterXml);
        }

        private XmlNode GetRootNode()
        {
            XmlNode node = _currentNode;
            while (node.ParentNode != null)
            {
                node = node.ParentNode;
            }

            return node;
        }
    }
}
