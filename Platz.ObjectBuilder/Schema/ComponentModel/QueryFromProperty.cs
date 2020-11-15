using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Schema
{
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
