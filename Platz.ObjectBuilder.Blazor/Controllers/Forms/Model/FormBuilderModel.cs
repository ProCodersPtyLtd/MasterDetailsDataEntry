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

        public static void CopyFrom(FormBuilderModel model, StoreForm form)
        {
            model.Name = form.Name;
            model.Schema = form.Schema;
            model.Datasource = form.Datasource;
            model.IsListForm = form.IsListForm;
            model.Fields = form.Fields.Values.Select(f => new FieldComponentModel(f)).ToList();
        }
    }
}
