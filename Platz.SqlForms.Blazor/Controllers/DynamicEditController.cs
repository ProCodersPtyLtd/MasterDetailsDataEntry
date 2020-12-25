using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms.Blazor
{
    public class DynamicEditController
    {
        protected readonly IDynamicEditFormDataProvider _dataProvider;
        protected IDynamicEditForm _form;
        protected int _id;
        protected Dictionary<string, FieldState> _fieldStates = new Dictionary<string, FieldState>();
        public string Error { get; private set; }

        public DynamicEditController(IDynamicEditFormDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public void SetParameters(IDynamicEditForm form, int id)
        {
            _form = form;
            _id = id;
        }

        public object GetItem()
        {
            return _dataProvider.GetItem(_form, _id);
        }

        public IEnumerable<DataField> GetFields()
        {
            var result = _dataProvider.GetFormFields(_form);
            return result;
        }

        public FieldState GetFieldState(DataField field)
        {
            if (!_fieldStates.ContainsKey(field.BindingProperty))
            {
                _fieldStates[field.BindingProperty] = new FieldState(field);
            }

            return _fieldStates[field.BindingProperty];
        }
    }
}
