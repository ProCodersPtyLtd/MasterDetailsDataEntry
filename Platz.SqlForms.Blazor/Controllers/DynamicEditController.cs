using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms.Blazor
{
    public class DynamicEditController
    {
        protected readonly IDynamicEditFormDataProvider _dataProvider;
        protected IDynamicEditForm _form;
        protected Dictionary<string, FieldState> _fieldStates = new Dictionary<string, FieldState>();
        public string Error { get; private set; }

        public DynamicEditController(IDynamicEditFormDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public void SetParameters(IDynamicEditForm form)
        {
            _form = form;
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
