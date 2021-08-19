using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Model
{
    public class FieldComponentModel
    {
        public FieldComponentModel()
        { 
        }

        public FieldComponentModel(StoreFormField f)
        {
            CopyFrom(this, f);
        }

        //public string Name { get; set; }
        //public string Binding { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
        public FieldComponentType ComponentType { get; set; }

        public StoreFormField StoreField { get; set; } = new StoreFormField();

        public List<FieldRuleModel> Rules { get; set; } = new List<FieldRuleModel>();

        public static void CopyFrom(FieldComponentModel model, StoreFormField field)
        {
            model.StoreField = field;
            model.Order = field.Order;
            model.ComponentType = GetComponentType(field.ControlType);
        }

        private static Dictionary<string, FieldComponentType> FieldComponentTypeMap = new Dictionary<string, FieldComponentType> 
        {
            { "TextEdit", FieldComponentType.TextEdit },
            { "DateEdit", FieldComponentType.DateEdit },
            { "NumberEdit", FieldComponentType.NumberEdit },
            { "Checkbox", FieldComponentType.Checkbox },
            { "Dropdown", FieldComponentType.Dropdown },
        };

        private static FieldComponentType GetComponentType(string controlType)
        {
            return FieldComponentTypeMap[controlType];
        }
    }

    public enum FieldComponentType
    {
        TextEdit,
        DateEdit,
        NumberEdit,
        Checkbox,
        Dropdown,
    }
}
