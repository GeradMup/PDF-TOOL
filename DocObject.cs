using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Merger
{
    public class DocObject
    {
        public DocObject(string name, string path) 
        {
            documentName = name;
            filePath = path;
        }

        public string documentName { get; set; }
        public string filePath { get; set; }
    }
}
