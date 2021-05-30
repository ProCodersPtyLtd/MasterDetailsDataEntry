using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public class DesignQueryObject
    {
        public bool IsSubQuery { get; set; }

        public StoreDefinition Table { get; set; }
        public IQueryModel Query { get; set; }

        public string Name 
        { 
            get { return IsSubQuery ? Query.Name : Table.Name; } 

            set
            {
                if (IsSubQuery)
                {
                    Query.Name = value;
                }
                else
                {
                    Table.Name = value;
                }
            }
        }

        public DesignQueryObject()
        { }

        public DesignQueryObject(IQueryModel query)
        {
            IsSubQuery = true;
            Query = query;
        }

        public DesignQueryObject(StoreDefinition table)
        {
            IsSubQuery = false;
            Table = table;
        }
    }
}
