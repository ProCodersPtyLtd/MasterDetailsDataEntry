using Platz.ObjectBuilder.Blazor.Model;
using Platz.SqlForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public interface IFormBuilderController
    {
        FormBuilderModel Model { get; set; }
        void Changed();

        List<FieldComponentModel> GetPageFieldComponents();
        void SetActive(FieldComponentModel field);
        void ReOrderFields(IList<FieldComponentModel> items);
        bool PageActive { get; }

        void SetSchemas(List<StoreSchema> storeSchemas);
        void SetQueries(List<StoreQuery> storeQueries);
        void RefreshDatasources();

        // Toolbar
        void AddTextEdit();
        void MoveUp();
        void MoveDown();
        void Clear();
        void GenerateFromEntity();
    }
    public class FormBuilderController : IFormBuilderController
    {
        //private List<FieldComponentModel> _fields;
        private List<StoreSchema> _storeSchemas;
        private List<StoreQuery> _storeQueries;

        public bool PageActive { get; set; }

        public FormBuilderModel Model { get; set; }

        public FormBuilderController()
        {
            // ToDo: remove this simulation
            Model = new FormBuilderModel();
            Model.Fields.Add(new FieldComponentModel { Name = "Name", Binding = "$.Name", ComponentType = FieldComponentType.TextEdit, StoreField = new StoreFormField() });
            Model.Fields.Add(new FieldComponentModel { Name = "Type", Binding = "$.Type", ComponentType = FieldComponentType.Dropdown, StoreField = new StoreFormField() });
            Model.Fields.Add(new FieldComponentModel { Name = "Created", Binding = "$.CreatedDate", ComponentType = FieldComponentType.DateEdit, StoreField = new StoreFormField() });
            
            ApplySortOrder();
        }

        public List<FieldComponentModel> GetPageFieldComponents()
        {
            return Model.Fields;
        }

        public void ReOrderFields(IList<FieldComponentModel> items)
        {
            // Actually Dropzone compnents reorders our _fields property
            Model.Fields = items.ToList();
            ApplySortOrder();
        }

        private void ApplySortOrder()
        {
            int i = 0;
            Model.Fields.ForEach(f => f.Order = i++);
        }

        public void SetActive(FieldComponentModel field)
        {
            Model.Fields.ForEach(f => f.Active = false);

            if (field == null)
            {
                PageActive = true;
                return;
            }

            PageActive = false;
            field.Active = true;
        }

        public void SetSchemas(List<StoreSchema> storeSchemas)
        {
            _storeSchemas = storeSchemas;
            Model.Schemas = _storeSchemas.Select(x => x.Name).ToList();
        }

        public void SetQueries(List<StoreQuery> storeQueries)
        {
            _storeQueries = storeQueries;
            //Model.Datasources = _storeQueries.Select(x => x.Name).ToList();
        }

        public void RefreshDatasources()
        {
            Model.Datasources = new List<string>();

            if (string.IsNullOrWhiteSpace(Model.Schema))
            {
                return;
            }

            Model.Datasources.AddRange(_storeQueries.Where(x => x.SchemaName == Model.Schema).Select(x => x.Name).OrderBy(x => x));

            Model.Datasources.AddRange(_storeSchemas.First(x => x.Name == Model.Schema).Definitions.Keys.OrderBy(x => x));

            if (Model.IsListForm)
            {

            }
            else
            {

            }
        }

        public void Changed()
        {
            Model.IsDirty = true;
        }

        public void AddTextEdit()
        {
            throw new NotImplementedException();
            Changed();
        }

        public void MoveUp()
        {
            throw new NotImplementedException();
            Changed();
        }

        public void MoveDown()
        {
            throw new NotImplementedException();
            Changed();
        }

        public void Clear()
        {
            Model.Fields.Clear();
            Changed();
        }

        public void GenerateFromEntity()
        {
            throw new NotImplementedException();
            Changed();
        }
    }
}
