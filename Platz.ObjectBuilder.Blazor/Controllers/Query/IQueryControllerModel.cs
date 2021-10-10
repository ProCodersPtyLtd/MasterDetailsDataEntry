using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public interface IQueryControllerModel
    {
        StoreQueryParameters StoreParameters { get; }
        //string Name { get; set; }
        StoreSchema Schema { get; }
        IQueryModel MainQuery { get; }
        List<IQueryModel> SubQueryList { get; }
        StringBuilder ErrorLog { get; }
    }
}
