using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Merger.Models
{
    public class DocObject(string path)
    {

        public string FilePath { get; set; } = path;
        public string DocumentName => Path.GetFileNameWithoutExtension(FilePath);
        
    }
}
