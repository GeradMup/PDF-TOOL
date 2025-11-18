using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Merger.Models
{
    public class RangeObject(int _start, int _end)
    {
        public int Start { get; private set; } = _start;
        public int End { get; private set; } = _end;
    }
}
