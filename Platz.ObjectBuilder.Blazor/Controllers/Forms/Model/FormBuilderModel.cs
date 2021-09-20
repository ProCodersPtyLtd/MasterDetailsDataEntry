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
        public string Namespace { get; set; }
        public string OriginalName { get; set; }
        public bool Validated { get; set; }
        public bool IsListForm { get; set; }
        public string Schema { get; set; }
        public List<string> Schemas { get; set; } = new List<string>();
        public string Datasource { get; set; }
        public List<string> Datasources { get; set; } = new List<string>();
        public List<string> HeaderParams { get; set; } = new List<string>();
        public List<string> QueryParams { get; set; } = new List<string>();
        public List<FieldComponentModel> Fields { get; set; } = new List<FieldComponentModel>();

        // Page Properties
        public string Caption { get; set; }
        public string RoutingPath { get; set; }
        public List<PageParameterModel> PageParameters { get; set; } = new List<PageParameterModel>();
        public string PageHeaderForm { get; set; }
        public bool PageHeaderFormReadOnly { get; set; }

        public string DisplayName
        {
            get
            {
                string dirty = IsDirty ? "*" : "";
                return $"{Name}{dirty}";
            }
        }

        public string GetRoutingPath()
        {
            if (!string.IsNullOrWhiteSpace(RoutingPath))
            {
                return RoutingPath;
            }

            return Name;
        }

        public static void CopyFrom(FormBuilderModel model, StoreForm form)
        {
            model.Name = form.Name;
            model.OriginalName = form.Name;
            model.Namespace = form.Namespace;
            model.Validated = form.Validated;
            model.Schema = form.Schema;
            model.Datasource = form.Datasource;
            model.IsListForm = form.IsListForm;
            model.Fields = form.Fields.OrderBy(x => x.Order).Select(f => new FieldComponentModel(f)).ToList();
            var buttons = form.ActionButtons.OrderBy(x => x.Order).Select(f => new FieldComponentModel(f)).ToList();
            model.Fields.AddRange(buttons);
            model.Caption = form.Caption;
            model.RoutingPath = form.RoutingPath;
            //model.PageHeaderForm = form.PageHeaderForm?.Name;
            model.PageHeaderForm = form.PageHeaderForm;
            model.PageHeaderFormReadOnly = form.PageHeaderFormReadOnly;
            model.PageParameters = form.PageParameters.OrderBy(x => x.Order).Select(f => new PageParameterModel(f)).ToList();
        }

        public StoreForm ToStore()
        {
            var src = this;
            var form = new StoreForm();
            form.Name = src.Name;
            form.Namespace = src.Namespace;
            form.Validated = src.Validated;
            form.Schema = src.Schema;
            form.Datasource = src.Datasource;
            form.IsListForm = src.IsListForm;
            form.Fields = src.Fields.Where(f => f.ComponentType != FieldComponentType.ActionButton).Select(f => f.ToStore()).ToList(); 
            form.ActionButtons = src.Fields.Where(f => f.ComponentType == FieldComponentType.ActionButton).Select(f => f.ToStoreButton()).ToList();
            form.Caption = src.Caption;
            form.RoutingPath = src.RoutingPath;
            form.PageHeaderForm = src.PageHeaderForm;
            form.PageHeaderFormReadOnly = src.PageHeaderFormReadOnly;
            form.PageParameters = src.PageParameters.Select(p => p.ToStore()).ToList();
            return form;
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

        public StorePageParameter ToStore()
        {
            var src = this;
            var par = new StorePageParameter();
            par.Name = src.Name;
            par.DataType = src.DataType;
            par.Order = src.Order;
            par.DatasourceQueryParameterMapping = src.DatasourceQueryParameterMapping;
            par.HeaderFormParameterMapping = src.HeaderFormParameterMapping;
            return par;
        }
    }

    
}
