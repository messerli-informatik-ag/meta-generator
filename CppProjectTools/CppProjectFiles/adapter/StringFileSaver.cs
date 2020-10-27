using System;
using System.Collections.Generic;
using System.Text;

namespace CppProjectFiles
{
    public class StringFileSaver : IStringSaver
    {
        private string _filepath;

        public StringFileSaver(string filepath) => _filepath = filepath;

        public void Save(string data)
        {
            System.IO.File.WriteAllText(_filepath, data);
        }
    }
}
