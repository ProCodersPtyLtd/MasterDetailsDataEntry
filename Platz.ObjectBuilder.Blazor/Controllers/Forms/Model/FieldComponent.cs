using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Model
{
    public class FieldComponent
    {
        public string Name { get; set; }
        public string Binding { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
        public FieldComponentType ComponentType { get; set; }

        public StoreFormField StoreField { get; set; }
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
