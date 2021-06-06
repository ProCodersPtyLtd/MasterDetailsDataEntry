using Platz.SqlForms;
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
        public bool IsSubQuery { get; private set; }

        //public StoreDefinition StoreDefinitionOld { get; set; }

        public List<QueryFromProperty> Properties { get; set; }

        // component fields
        public string Alias { get; set; }
        public string Name { get; private set; }

        public QueryFromTable()
        {
            Id = _counter;
            _counter++;
        }

        public QueryFromTable(StoreDefinition source): this()
        {
            CopyFrom(source);
        }

        public QueryFromTable(IQueryModel source) : this()
        {
            CopyFrom(source);
        }

        public void CopyFrom(StoreDefinition source)
        {
            //StoreDefinitionOld = source;
            Name = source.Name;
            Properties = source.Properties.Values.Select(p => new QueryFromProperty(p)).ToList();
        }

        public void CopyFrom(IQueryModel source)
        {
            IsSubQuery = true;
            Name = source.Name;
            //StoreDefinition = source;
            Properties = source.SelectionProperties.Select(p => new QueryFromProperty(p)).ToList();
        }
    }

    
}
