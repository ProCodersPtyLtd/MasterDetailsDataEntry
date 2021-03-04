using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public class DiagramTable
    {
        private static int _counter;

        public int Id { get; private set; }

        public string Name { get; set; }
        public List<Column> Columns { get; set; }

        public DiagramTable()
        {
            Id = _counter;
            _counter++;
        }
    }

    public class Column
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsPk { get; set; }
        public bool IsFk { get; set; }
        public string FkTable { get; set; }
        public string FkColumn { get; set; }
    }
}
