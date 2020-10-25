using MasterDetailsDataEntry.Shared;
using MasterDetailsDataEntry.Shared.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public class DefaultDataFieldProcessor : IDataFieldProcessor
    {
        public List<DataField> PrepareFields(List<DataField> list, Type type)
        {
            foreach (var field in list)
            {
                ResolveControlType(type, field);
                ResolveLabel(type, field);
            }

            return list;
        }

        public void ResolveLabel(Type type, DataField field)
        {
            if (field.Label == null)
            {
                field.Label = field.BindingProperty;
            }
        }

        public void ResolveControlType(Type type, DataField field)
        {
            var property = type.GetProperty(field.BindingProperty);
            field.DataType = property.PropertyType;

            if (field.ControlType == null)
            {
                var dataTypeName = field.DataType.Name;

                if (field.DataType.Name == "Nullable`1")
                {
                    dataTypeName = Nullable.GetUnderlyingType(field.DataType).Name;
                }

                switch (dataTypeName)
                {
                    case "Boolean":
                        field.ControlType = typeof(DefaultCheckboxControl);
                        break;
                    case "DateTime":
                        field.ControlType = typeof(DefaultDateEditControl);
                        break;
                    default:
                        field.ControlType = typeof(DefaultTextEditControl);
                        break;
                }
            }
        }
    }
}
