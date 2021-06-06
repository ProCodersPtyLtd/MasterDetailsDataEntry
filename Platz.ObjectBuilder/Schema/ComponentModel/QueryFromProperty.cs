using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Schema
{
    public class QueryFromProperty
    {
        public string Name { get; private set; }
        //public string OriginalName { get; private set; }
        public bool Pk { get; private set; }
        public bool Fk { get; private set; }

        public StoreProperty OriginalStoreProperty { get; set; }

        // component fields
        // ToDo: Alias should be removed as it is not used, only in tests
        public string Alias { get; set; }
        public bool Selected { get; set; }

        public QueryFromProperty() { }

        public QueryFromProperty(StoreProperty source)
        {
            OriginalStoreProperty = source;
            Name = source.Name;
            Pk = source.Pk;
            Fk = source.Fk;
            //StoreProperty = source;
        }

        public QueryFromProperty(QuerySelectProperty source)
        {
            OriginalStoreProperty = new SubQueryStoreProperty(source);
            //OriginalStoreProperty = source.StoreProperty;
            //OriginalName = source.StoreProperty.Name;
            Name = source.OutputName;
            //StoreProperty = source;
        }
    }
}
