using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetsExporter
{
    public class WorksheetValues
    {
        public IEnumerable<Dictionary<string, string>> Values { get; set; }
    }
}
