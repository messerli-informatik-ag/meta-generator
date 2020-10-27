using System;
using System.Collections.Generic;
using System.Text;

namespace CppProjectFiles
{
    public struct ProjectFileClassParams
    {
        public IStringLoader XmlLoader;
        public IStringSaver XmlSaver;
        public IXmlWalker XmlWalker;
        public IXmlDataAdder XmlDataAdder;
    }
}
