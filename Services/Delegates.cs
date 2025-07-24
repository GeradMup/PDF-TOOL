using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Merger.Services
{
    public class Delegates
    {
        public delegate void OnMergeComplete(string mergedFilePath);
        public delegate void OnSplitComplete(string splitFilePath);
    }
}
