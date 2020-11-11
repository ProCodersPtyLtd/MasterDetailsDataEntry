using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Schema
{
    public class QueryFromTable
    {
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
        { }

        public QueryFromTable(StoreDefinition source)
        {
            CopyFrom(source);
        }
    }

    public class QueryFromProperty
    {
        public StoreProperty StoreProperty { get; set; }

        // component fields
        public string Alias { get; set; }
        public bool Selected { get; set; }

        public QueryFromProperty() { }

        public QueryFromProperty(StoreProperty source)
        {
            StoreProperty = source;
        }

    }
}
