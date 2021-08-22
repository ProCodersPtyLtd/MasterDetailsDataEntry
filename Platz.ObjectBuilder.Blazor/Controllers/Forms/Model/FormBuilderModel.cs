using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder.Blazor.Model
{
    public class FormBuilderModel
    {
        public FormBuilderModel()
        {

        }

        public FormBuilderModel(StoreForm item)
        {
            CopyFrom(this, item);
        }

        public bool IsDirty { get; set; }
        public string Name { get; set; }
        public bool IsListForm { get; set; }
        public string Schema { get; set; }
        public List<string> Schemas { get; set; } = new List<string>();
        public string Datasource { get; set; }
        public List<string> Datasources { get; set; } = new List<string>();
        public List<FieldComponentModel> Fields { get; set; } = new List<FieldComponentModel>();

        // Page Properties
        public string PagePath { get; set; }
        public List<PageParameterModel> PageParameters { get; set; } = new List<PageParameterModel>();
        public string PageHeaderForm { get; set; }

        public string DisplayName
        {
            get
            {
                string dirty = IsDirty ? "*" : "";
                return $"{Name}{dirty}";
            }
        }

        public static void CopyFrom(FormBuilderModel model, StoreForm form)
        {
            model.Name = form.Name;
            model.Schema = form.Schema;
            model.Datasource = form.Datasource;
            model.IsListForm = form.IsListForm;
            model.Fields = form.Fields.Values.Select(f => new FieldComponentModel(f)).ToList();
            model.PagePath = form.PagePath;
            model.PageHeaderForm = form.PageHeaderForm?.Name;
            model.PageParameters = form.PageParameters.Values.Select(f => new PageParameterModel(f)).ToList();
        }
    }

    public class PageParameterModel
    {
        public static readonly string[] DataTypes = new string[] { "string", "int", "date", "decimal", "bool", "guid" };

        public string Name { get; set; }
        public string DataType { get; set; }
        public int Order { get; set; }

        // Mapping to Query StoreQueryParameter.Name of Datasource, can be null if not mapped
        public string DatasourceQueryParameterMapping { get; set; }

        // Mapping to parameter in HeaderForm
        public string HeaderFormParameterMapping { get; set; }

        public PageParameterModel()
        {
        }

        public PageParameterModel(StorePageParameter item)
        {
            CopyFrom(this, item);
        }

        public static void CopyFrom(PageParameterModel model, StorePageParameter item)
        {
            model.Name = item.Name;
            model.DataType = item.DataType;
            model.Order = item.Order;
            model.DatasourceQueryParameterMapping = item.DatasourceQueryParameterMapping;
            model.HeaderFormParameterMapping = item.HeaderFormParameterMapping;
        }
    }

    
}
