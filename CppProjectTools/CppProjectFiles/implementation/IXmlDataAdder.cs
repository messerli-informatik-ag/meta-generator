using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CppProjectFiles
{
    public interface IXmlDataAdder
    {
        void SetData(XmlNode node, string value);

        void SetAttribute(XmlNode node, string attribute, string value);

        void AddNode(XmlNode node, string name);
    }
}
