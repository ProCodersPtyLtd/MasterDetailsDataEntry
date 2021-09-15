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

        public List<StoreFormField> Fields { get; set; } = new List<StoreFormField>();
        public List<StoreFormButton> ActionButtons { get; set; } = new List<StoreFormButton>();

        // Page Properties
        public string PagePath { get; set; }
        public List<StorePageParameter> PageParameters { get; set; } = new List<StorePageParameter> ();
        public string PageHeaderForm { get; set; }
        public bool Validated { get; set; }
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
        public int Order { get; set; }
        public bool Hidden { get; set; }
        public bool? ReadOnly { get; set; }
        //public string LinkText { get; set; }
        public string NavigationTargetForm { get; set; }
        public List<StoreNavigationParameter> NavigationParameterMapping { get; set; } = new List<StoreNavigationParameter>();
    }

    public class StoreNavigationParameter
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public int Order { get; set; }
        public string SupplyingParameterMapping { get; set; }

    }
}
