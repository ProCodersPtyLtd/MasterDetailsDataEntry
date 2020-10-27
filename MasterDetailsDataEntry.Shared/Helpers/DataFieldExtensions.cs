using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public static class DataFieldExtensions
    {
        public static int GetPrimaryKeyValue(this object item, IEnumerable<DataField> fields)
        {
            var id = (int)item.GetPropertyValue(fields.Single(f => f.PrimaryKey).BindingProperty);
            return id;
        }

        public static void SetForeignKeyValue(this object item, IEnumerable<DataField> fields, int? id)
        {
            if (id != null && fields.Any(f => f.ForeignKey))
            {
                item.SetPropertyValue(fields.First(f => f.ForeignKey).BindingProperty, id.Value);
            }
        }
    }
}
