using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public class StoreForm : IStoreObject
    {
        public string Name { get; set; }
        public bool IsListForm { get; set; }
        public string Schema { get; set; }
        public string Datasource { get; set; }

        public Dictionary<string, StoreFormField> Fields { get; set; } = new Dictionary<string, StoreFormField>();
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
        public int Order { get; set; }
        public FieldFilterType FilterType { get; set; }
    }
}
