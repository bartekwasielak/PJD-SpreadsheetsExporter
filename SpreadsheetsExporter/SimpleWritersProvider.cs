using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetsExporter
{
    public class SimpleWritersProvider : IWritersProviders
    {
        private List<Writer> _writers;

        public SimpleWritersProvider()
        {
            _writers = new List<Writer> {
               new Writers.Przychody(),
               new Writers.EwidencjaOZ()
            };
        }

        public IEnumerable<WriterInfo> GetWriters()
        {
            return _writers.Select(w => new WriterInfo() { WriterName = w.Name });
        }

        public Writer GetWriter(string name)
        {
            var writer = _writers.First(w => w.Name == name);
            return writer;
        }
    }
}
