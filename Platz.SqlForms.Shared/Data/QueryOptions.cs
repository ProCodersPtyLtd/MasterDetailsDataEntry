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
        public List<FieldFilter> Filters { get; set; } = new List<FieldFilter>();
    }

    public enum SortDirection
    {
        None,
        Asc,
        Desc
    }

    public class FieldFilter
    {
        public string BindingProperty { get; set; }
        public string Filter { get; set; }
        public FieldFilterType FilterType { get; set; }
    }
}
