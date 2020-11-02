using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms.Shared
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
            var dataTypeName = field.DataType.Name;

            if (field.DataType.Name == "Nullable`1")
            {
                dataTypeName = Nullable.GetUnderlyingType(field.DataType).Name;
            }

            switch (dataTypeName)
            {
                case "Int32":
                case "Decimal":
                case "Float":
                case "Double":
                    field.ControlType = field.ControlType ?? typeof(DefaultNumberEditControl);
                    field.ViewModeControlType = field.ViewModeControlType ?? typeof(DefaultFormattedViewControl);
                    break;
                case "Boolean":
                    field.ControlType = field.ControlType ?? typeof(DefaultCheckboxControl);
                    field.ViewModeControlType = field.ViewModeControlType ?? typeof(DefaultReadonlyCheckboxControl);
                    break;
                case "DateTime":
                    field.ControlType = field.ControlType ?? typeof(DefaultDateEditControl);
                    field.ViewModeControlType = field.ViewModeControlType ?? typeof(DefaultFormattedViewControl);
                    break;
                default:
                    field.ControlType = field.ControlType ?? typeof(DefaultTextEditControl);
                    field.ViewModeControlType = field.ViewModeControlType ?? typeof(DefaultFormattedViewControl);
                    break;
            }
        }
    }
}
