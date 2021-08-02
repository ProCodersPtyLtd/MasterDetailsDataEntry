using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public class StoreForm
    {
        public string Name { get; set; }

        public Dictionary<string, StoreFormField> Fields { get; set; }
    }

    public class StoreFormField
    {
        public string BindingProperty { get; set; }
        public string DataType { get; set; }
        public string ControlType { get; set; }
        public string ViewModeControlType { get; set; }
        public string Label { get; set; }
        public bool Required { get; set; }
        public bool Hidden { get; set; }
        public bool? ReadOnly { get; set; }
        public string Format { get; set; }
        public FieldFilterType FilterType { get; set; }
    }
}
