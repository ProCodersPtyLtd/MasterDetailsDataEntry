using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms.Blazor
{
    public class DynamicEditController
    {
        protected readonly IDataValidationProvider _dataValidationProvider;
        protected readonly IDynamicEditFormDataProvider _dataProvider;
        protected IDynamicEditForm _form;
        protected int _id;
        protected Dictionary<string, FieldState> _fieldStates = new Dictionary<string, FieldState>();

        public object ModelItem { get; private set; }
        public IEnumerable<DataField> Fields { get; private set; }
        public IEnumerable<DialogButtonDetails> Buttons { get; private set; }
        public string Error { get; private set; }

        public DynamicEditController(IDynamicEditFormDataProvider dataProvider, IDataValidationProvider dataValidationProvider)
        {
            _dataProvider = dataProvider;
            _dataValidationProvider = dataValidationProvider;
        }

        public void SetParameters(IDynamicEditForm form, int id)
        {
            _form = form;
            _id = id;
            Fields = GetFields();
            ModelItem = GetItem();
            Buttons = _form.GetButtons();
        }

        public object GetItem()
        {
            var result = _dataProvider.GetItem(_form, _id) ?? CreateItem();
            return result;
        }

        private object CreateItem()
        {
            var type = _form.GetEntityType();
            var result = Activator.CreateInstance(type);
            return result;
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

            var result = _fieldStates[field.BindingProperty];
            result.Value = GetItemValue(field);
            return result;
        }

        public void LocalModelValueChanged(ValueChangedArgs args)
        {
            SetItemValue(args.Field, args.State);

            if (args.State.ValidationMessages.Any())
            {
                var validations = _dataValidationProvider.ValidateModelProperty(_form, ModelItem, args.State.RowIndex, args.Field.BindingProperty, Fields);
                UpdateFieldStateValidations(validations, args.State.RowIndex, args.Field.BindingProperty);
            }
        }

        private void UpdateFieldStateValidations(IEnumerable<ValidationResult> validations, int rowIndex, string bindingProperty = null)
        {
            //if (bindingProperty == null)
            //{
            //    ClearValidations();
            //}
            //else
            //{
            //    var key = GetFieldStateKey(bindingProperty, rowIndex);
            //    _states[key].ValidationMessages = new List<string>();
            //}

            //foreach (var validation in validations)
            //{
            //    if (bindingProperty == null || validation.BindingProperty == bindingProperty)
            //    {
            //        if (validation.ValidationResultType == ValidationResultTypes.Error)
            //        {
            //            var key = GetFieldStateKey(validation.BindingProperty, validation.RowIndex);
            //            _states[key].ValidationMessages.Add(validation.Message);
            //        }
            //    }
            //}
        }

        #region reflection get set property
        public object GetItemValue(DataField field)
        {
            var item = ModelItem;
            var result = item.GetPropertyValue(field.BindingProperty);
            return result;
        }

        public void SetItemValue(DataField field, FieldState state)
        {
            var item = ModelItem;
            item.SetPropertyValue(field.BindingProperty, state.Value);
        }
        #endregion
    }
}
