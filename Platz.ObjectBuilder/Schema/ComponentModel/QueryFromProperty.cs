using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Schema
{
    public class QueryFromProperty
    {
        // component fields
        public string Alias { get; set; }
        public bool Selected { get; set; }
        public string Name { get { return OriginalStoreProperty.Name; } }
        public bool Pk { get { return OriginalStoreProperty.Pk; } }
        public bool Fk { get { return OriginalStoreProperty.Fk; } }
        public StoreProperty OriginalStoreProperty { get; set; }

        public QueryFromProperty() { }

        public QueryFromProperty(StoreProperty source)
        {
            OriginalStoreProperty = source;
        }

        public QueryFromProperty(QuerySelectProperty source)
        {
            OriginalStoreProperty = new SubQueryStoreProperty(source);
        }
    }
}
