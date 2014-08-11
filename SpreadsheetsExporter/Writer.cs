using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetsExporter
{
    public abstract class Writer
    {
        class NullComparer : IComparer<Dictionary<string, string>>
        {
            public int Compare(Dictionary<string, string> x, Dictionary<string, string> y)
            {
                return 0;
            }
        }

        public abstract string Name { get; }

        protected abstract string WriteRow(Dictionary<string, string> row);
        protected abstract string WriteEnd();
        protected abstract string WriteBegin();
        protected IComparer<Dictionary<string, string>> RowComparer = new NullComparer();

        public string Write(WorksheetValues values)
        {
            var sb = new StringBuilder();

            sb.Append(WriteBegin());

            var ordered = values.Values.OrderBy(a => a, RowComparer).ToList();

            foreach (var row in ordered)
	        {
                sb.AppendLine(WriteRow(row));
	        }

            sb.Append(WriteEnd());

            return sb.ToString();
        }
    }
}
