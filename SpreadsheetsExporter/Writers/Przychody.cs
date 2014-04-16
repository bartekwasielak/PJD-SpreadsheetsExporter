using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pluralize;

namespace SpreadsheetsExporter.Writers
{
    public class Przychody : Writer
    {
        const string lancetyForms = "lancet|lancety|lancetów|lanceta";

        public override string Name
        {
            get { return "Przychody"; }
        }

        private decimal _sum = 0.0m;

        protected override string WriteBegin()
        {
            _sum = 0.0m;
            return string.Empty;
        }

        protected override string WriteRow(Dictionary<string, string> row)
        {
            decimal lancetyDec;
            var lancety = decimal.TryParse(row["lancetyzebrane"], out lancetyDec);
            var result = string.Format("{0}. {1}, {2}{3}, od: {4}{5}{6}",
                row["lp"], row["data"], row["lancetyzebrane"],
                lancety ? (" " + lancetyForms.Pluralize(lancetyDec)) : "",
                row["kto"].Trim(),
                string.IsNullOrEmpty(row["zaco"]) ? "" : (", " + row["zaco"].Trim()),
                string.IsNullOrEmpty(row["najakicel"]) ? "" : (", " + row["najakicel"].Trim()));
            if (lancety)
            {
                _sum += lancetyDec;
            }
            return result;
        }

        protected override string WriteEnd()
        {
            return Environment.NewLine + 
                string.Format("[b][size=150]Zebrany podatek to {0} {1}, czyli {2} zł!!![/size][/b]",
                _sum, lancetyForms.Pluralize(_sum), Math.Round(_sum * 10, 2))
                + Environment.NewLine;
        }
    }
}
