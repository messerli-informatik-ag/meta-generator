using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CppProjectFiles
{
    public class XmlFileWalker : IXmlWalker
    {
        private XmlFileWalkerClassParams _classParams;

        public XmlFileWalker(XmlFileWalkerClassParams classparams) => _classParams = classparams;

        public XmlNode VisitNode(XmlDocument doc, string nodeName)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace(_classParams.RootNodename, _classParams.RootNamespace);
            return doc.DocumentElement.SelectSingleNode(_classParams.WorkingNode, nsmgr);
        }
    }
}
