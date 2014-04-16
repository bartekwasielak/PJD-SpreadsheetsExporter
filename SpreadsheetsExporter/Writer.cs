using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetsExporter
{
    public abstract class Writer
    {
        public abstract string Name { get; }

        protected abstract string WriteRow(Dictionary<string, string> row);
        protected abstract string WriteEnd();
        protected abstract string WriteBegin();

        public string Write(WorksheetValues values)
        {
            var sb = new StringBuilder();

            sb.Append(WriteBegin());

            foreach (var row in values.Values)
	        {
                sb.AppendLine(WriteRow(row));
	        }

            sb.Append(WriteEnd());

            return sb.ToString();
        }
    }
}
