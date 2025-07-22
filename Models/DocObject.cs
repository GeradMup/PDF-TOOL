using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Merger.Models
{
    public class DocObject(string name, string path)
    {
        public string DocumentName { get; set; } = name;
        public string FilePath { get; set; } = path;
    }
}
