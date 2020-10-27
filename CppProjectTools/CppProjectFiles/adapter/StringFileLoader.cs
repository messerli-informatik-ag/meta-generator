using System;
using System.Collections.Generic;
using System.Text;

namespace CppProjectFiles
{
    /// <summary>
    /// Load text from file.
    /// </summary>
    public class StringFileLoader : IStringLoader
    {
        private readonly string _filepath;

        public StringFileLoader(string filepath) => _filepath = filepath;

        public string GetData()
        {
            return System.IO.File.ReadAllText(_filepath);
        }
    }
}
