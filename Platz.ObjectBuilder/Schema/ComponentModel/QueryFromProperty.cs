using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Schema
{
    public class QueryFromProperty
    {
        public StoreProperty StoreProperty { get; set; }

        // component fields
        // ToDo: Alias should be removed as it is not used, only in tests
        public string Alias { get; set; }
        public bool Selected { get; set; }

        public QueryFromProperty() { }

        public QueryFromProperty(StoreProperty source)
        {
            StoreProperty = source;
        }

    }
}
