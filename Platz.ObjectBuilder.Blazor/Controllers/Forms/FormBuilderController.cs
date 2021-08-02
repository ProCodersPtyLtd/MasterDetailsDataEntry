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
        FormBuilderModel BuilderModel { get; set; }

        List<FieldComponent> GetPageFieldComponents();
        void SetActive(FieldComponent field);
        void ReOrderFields(IList<FieldComponent> items);
    }
    public class FormBuilderController : IFormBuilderController
    {
        private List<FieldComponent> _fields;

        public FormBuilderModel BuilderModel 
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public FormBuilderController()
        {
            // ToDo: remove this simulation
            _fields = new List<FieldComponent>();
            _fields.Add(new FieldComponent { Name = "Name", Binding = "$.Name", ComponentType = FieldComponentType.TextEdit, StoreField = new StoreFormField() });
            _fields.Add(new FieldComponent { Name = "Type", Binding = "$.Type", ComponentType = FieldComponentType.Dropdown, StoreField = new StoreFormField() });
            _fields.Add(new FieldComponent { Name = "Created", Binding = "$.CreatedDate", ComponentType = FieldComponentType.DateEdit, StoreField = new StoreFormField() });
            
            ApplySortOrder();
        }

        public List<FieldComponent> GetPageFieldComponents()
        {
            return _fields;
        }

        public void ReOrderFields(IList<FieldComponent> items)
        {
            // Actually Dropzone compnents reorders our _fields property
            _fields = items.ToList();
            ApplySortOrder();
        }

        private void ApplySortOrder()
        {
            int i = 0;
            _fields.ForEach(f => f.Order = i++);
        }

        public void SetActive(FieldComponent field)
        {
            _fields.ForEach(f => f.Active = false);
            field.Active = true;
        }
    }
}
