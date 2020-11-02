using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms.Shared
{
    public static class DataFieldExtensions
    {
        public static int GetPrimaryKeyValue(this object item, IEnumerable<DataField> fields)
        {
            var id = (int)item.GetPropertyValue(fields.Single(f => f.PrimaryKey).BindingProperty);
            return id;
        }

        public static void SetFilterKeyValue(this object item, IEnumerable<DataField> fields, int? id)
        {
            if (id != null && fields.Any(f => f.Filter))
            {
                item.SetPropertyValue(fields.First(f => f.Filter).BindingProperty, id.Value);
            }
        }
    }
}
