using System;
using System.Collections.Generic;
using System.Text;

namespace CppProjectFiles
{
    public interface IXmlFile : IDisposable
    {
        IXmlFile GotoNode(string node);

        /// <summary>
        /// Searches for nodeValue at the current node.
        /// </summary>
        /// <param name="nodeValue">The value to be searched for.</param>
        /// <returns>True, if nodeValue is found in the content of the current node, false otherwise.</returns>
        bool Contains(string nodeValue);

        /// <summary>
        /// Sets new node value, overwriting its content.
        /// </summary>
        /// <param name="nodeValue">The data to be set.</param>
        void SetData(string nodeValue);

        void SetAttribute(string attribute, string value);

        void AddNode(string nodeName);

        string GetData();
    }
}
