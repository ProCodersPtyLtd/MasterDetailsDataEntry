using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Schema
{
    public class QuerySelectProperty
    {
        public StoreProperty StoreProperty { get; set; }
        public QueryFromTable FromTable { get; set; }

        // component fields
        public bool IsOutput { get; set; }
        public string Filter { get; set; }
        public string GroupByFunction { get; set; }
        public bool IsGroup { get; set; }
        public string Alias { get; set; }

        public string OutputName {  get { return string.IsNullOrWhiteSpace(Alias) ? StoreProperty.Name : Alias; } }

        public QuerySelectProperty() { }

        public QuerySelectProperty(QueryFromTable fromTable, StoreProperty source)
        {
            FromTable = fromTable;
            StoreProperty = source;
        }
    }
}
