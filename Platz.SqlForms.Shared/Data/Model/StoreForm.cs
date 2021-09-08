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
        public Dictionary<string, StoreFormButton> Buttons { get; set; } = new Dictionary<string, StoreFormButton>();

        // Page Properties
        public string PagePath { get; set; }
        public Dictionary<string, StorePageParameter> PageParameters { get; set; } = new Dictionary<string, StorePageParameter>();
        public string PageHeaderForm { get; set; }
    }

    public class StorePageParameter
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public int Order { get; set; }

        // Mapping to Query StoreQueryParameter.Name of Datasource, can be null if not mapped
        public string DatasourceQueryParameterMapping { get; set; }

        // Mapping to parameter in HeaderForm
        public string HeaderFormParameterMapping { get; set; }
    }

    public class StoreFormField
    {
        public string BindingProperty { get; set; }
        public string DataType { get; set; }
        public string ControlType { get; set; }
        // ControlReadOnly
        public string ViewModeControlType { get; set; }
        public string Label { get; set; }
        public bool Required { get; set; }
        public bool Hidden { get; set; }
        public bool? ReadOnly { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Unique { get; set; }
        public string Format { get; set; }
        public int Order { get; set; }
        public bool Filter { get; set; }
        public FieldFilterType FilterType { get; set; }
    }

    public class StoreFormButton
    {
        public string Action { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }
        public string LinkText { get; set; }
        public int Order { get; set; }
    }
}
