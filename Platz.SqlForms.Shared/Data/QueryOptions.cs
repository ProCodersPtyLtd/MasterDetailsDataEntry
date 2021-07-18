using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public class QueryOptions
    {
        public string SortColumn { get; set; }
        public SortDirection SortDirection { get; set; }
    }

    public enum SortDirection
    {
        None,
        Asc,
        Desc
    }
}
