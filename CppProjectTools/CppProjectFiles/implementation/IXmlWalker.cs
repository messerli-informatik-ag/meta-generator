using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CppProjectFiles
{
    public interface IXmlWalker
    {
        XmlNode VisitNode(XmlDocument doc, string nodeName);
    }
}
