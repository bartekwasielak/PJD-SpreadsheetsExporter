using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pluralize;

namespace SpreadsheetsExporter.Writers
{
    public class EwidencjaOZ : Writer
    {
        class EwidencjaOZComparer : IComparer<Dictionary<string, string>>
        {
            public int Compare(Dictionary<string, string> x, Dictionary<string, string> y)
            {
                DateTime datex, datey;
                var datexExists = DateTime.TryParse(x["datawykonaniazabiegukastracji"], out datex);
                var dateyExists = DateTime.TryParse(y["datawykonaniazabiegukastracji"], out datey);
                if (datexExists && dateyExists)
                {
                    return datex.CompareTo(datey);
                }
                if (datexExists)
                {
                    return -1;
                }
                if (dateyExists)
                {
                    return 1;
                }
                return 0;
            }
        }

        public EwidencjaOZ()
        {
            RowComparer = new EwidencjaOZComparer();
        }

        public override string Name
        {
            get { return "Ewidencja OZ"; }
        }

        protected override string WriteBegin()
        {
            nowyNrNaMiau = 1;
            currentYear = 2010;
            return string.Empty;
        }

        int nowyNrNaMiau = 1;
        private int currentYear = 2010;
        private string div = Environment.NewLine + "-----------------------tu tnij post -----------------------" + Environment.NewLine;

        protected override string WriteRow(Dictionary<string, string> row)
        {
            var putDiv = false;
            if (row["nrnamiau"].Trim() == "brak")
            {
                return string.Empty;
            }

            var dateString = row["datawykonaniazabiegukastracji"];
            DateTime date;
            if (DateTime.TryParse(dateString, out date))
            {
                if (date.Year != currentYear)
                {
                    putDiv = true;
                    currentYear = date.Year;
                }
                dateString = date.ToString("dd-MM-yyyy");
            }
            var result = string.Format("{0}. {1} {2}płeć: {3}, kolor: {4}, status: {5}, miejscowość: {6}{7}{8}{9}", 
                nowyNrNaMiau.ToString(), 
                dateString,
                (string.IsNullOrEmpty(row["imiekota"]) || row["imiekota"].StartsWith("-")) ? "" : row["imiekota"].Trim() + ", ",
                row["płeć"].Trim(),
                row["koloriznakiszczególne"].Trim(),
                row["statuskota"].Trim(),
                row["miejscebytowania"].Trim(),
                !string.IsNullOrEmpty(row["operator"]) ? string.Format(" ({0})", row["operator"].Trim()) : string.Empty,
                (row["uwagi"].ToLowerInvariant().Contains("kongres") ? ", ze środków BIL, Kongres Kobiet" : string.Empty) + 
                ((row["uwagi"].Contains("1%") || row["numerrachunkufaktury"].Contains("1%")) ? ", ze środków zebranych w ramach 1%" : string.Empty),
                (!string.IsNullOrEmpty(row["zdjęcie"]) && row["zdjęcie"].StartsWith("http") && row["publikacjazdjęcia"] == "tak") ? 
                    string.Format("{0}[url={1}][img]{2}[/img][/url]", 
                        Environment.NewLine, 
                        row["zdjęcie"],
                        row["zdjęcie"].Replace("http://www.podjednymdachem.org/upload/", "http://www.podjednymdachem.org/upload/m/"))
                    : string.Empty
                );
            nowyNrNaMiau++;
            return (putDiv ? div + Environment.NewLine : string.Empty) + result;
        }

        protected override string WriteEnd()
        {
            return string.Empty;
        }
    }
}
