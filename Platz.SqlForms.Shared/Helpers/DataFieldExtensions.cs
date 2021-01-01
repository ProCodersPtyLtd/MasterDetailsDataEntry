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

        public static object[] GetFilterKeyValues(this object item, IEnumerable<DataField> fields)
        {
            var result = fields.Where(f => f.Filter).OrderBy(f => f.Order).Select(f => item.GetPropertyValue(f.BindingProperty)).ToArray();
            return result;
        }

        public static object[] GetPrimaryAndFilterKeyValues(this object item, IEnumerable<DataField> fields)
        {
            var result = new List<object>();
            result.Add(GetPrimaryKeyValue(item, fields));
            result.AddRange(GetFilterKeyValues(item, fields));
            return result.ToArray();
        }

        public static void SetFilterKeyValue(this object item, IEnumerable<DataField> fields, int? id)
        {
            if (id != null && fields.Any(f => f.Filter))
            {
                item.SetPropertyValue(fields.First(f => f.Filter).BindingProperty, id.Value);
            }
        }

        public static void SetFilterKeyValue(this object item, DataField field, object value)
        {
            if (value != null && field.Filter)
            {
                item.SetPropertyValue(field.BindingProperty, value);
            }
        }

        public static void SetFilterKeyValues(this object item, IEnumerable<DataField> fields, object[] values)
        {
            var filterFields = fields.Where(f => f.Filter).ToList();

            for (int i = 0; i < filterFields.Count; i++)
            {
                item.SetFilterKeyValue(filterFields[i], values[i]);
            }
        }
    }
}
