using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public class QueryControllerModel : IQueryControllerModel
    {
        public StoreQueryParameters StoreParameters { get; set; }
        //string Name { get; set; }
        public StoreSchema Schema { get; set; }
        public IQueryModel MainQuery { get; set; }
        public List<IQueryModel> SubQueryList { get; set; }
        public StringBuilder ErrorLog { get; set; } = new StringBuilder();
        public bool IsDirty { get; set; }

        public string DisplayName
        {
            get
            {
                string dirty = IsDirty ? "*" : "";
                return $"{StoreParameters.QueryName}{dirty}";
            }
        }

        public QueryControllerModel()
        { }

        public QueryControllerModel(StoreQuery src)
        { }

    }
}
