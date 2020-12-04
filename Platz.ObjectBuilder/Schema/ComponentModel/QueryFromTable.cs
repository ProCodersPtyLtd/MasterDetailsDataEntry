using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Schema
{
    public class QueryFromTable
    {
        private static int _counter;

        public int Id { get; private set; }

        public StoreDefinition StoreDefinition { get; set; }

        public List<QueryFromProperty> Properties { get; set; }

        // component fields
        public string Alias { get; set; }

        public void CopyFrom(StoreDefinition source)
        {
            StoreDefinition = source;
            Properties = source.Properties.Values.Select(p => new QueryFromProperty(p)).ToList();
        }

        public QueryFromTable()
        {
            Id = _counter;
            _counter++;
        }

        public QueryFromTable(StoreDefinition source): this()
        {
            CopyFrom(source);
        }
    }

    
}
