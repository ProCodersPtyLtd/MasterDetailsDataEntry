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
        //public string Name { get; set; }
        //public string Binding { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
        public FieldComponentType ComponentType { get; set; }

        public StoreFormField StoreField { get; set; } = new StoreFormField();
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
