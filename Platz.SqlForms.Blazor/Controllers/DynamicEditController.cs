using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms.Blazor
{
    public class DynamicEditController
    {
        protected readonly IDataValidationProvider _dataValidationProvider;
        protected readonly IDynamicEditFormDataProvider _dataProvider;
        protected IDynamicEditForm _form;
        protected int _id;
        protected object[] _serviceParameters;
        protected Dictionary<string, FieldState> _fieldStates = new Dictionary<string, FieldState>();
        private IEnumerable<ValidationResult> _validations = new ValidationResult[0];
        private Func<Task> _dataChanged;

        public object ModelItem { get; private set; }
        public IEnumerable<DataField> Fields { get; private set; }
        public IEnumerable<DialogButtonDetails> Buttons { get; private set; }
        public IEnumerable<DialogButtonNavigationDetails> ButtonNavigations { get; private set; }
        public object[] NavigateFormatParameters { get; private set; }
        public FormParameter[] FormParameters { get; private set; }
        public string Error { get; private set; }

        public DynamicEditController(IDynamicEditFormDataProvider dataProvider, IDataValidationProvider dataValidationProvider)
        {
            _dataProvider = dataProvider;
            _dataValidationProvider = dataValidationProvider;
        }

        public void SetParameters(IDynamicEditForm form, FormParameter[] formParameters, Func<Task> dataChanged)
        {
            FormParameters = formParameters;
            int id = (int)formParameters[0].Value;
            var serviceParameters = formParameters.Skip(1).Select(p => p.Value).ToArray();
            SetParameters(form, id, serviceParameters, dataChanged);
        }

        public void SetParameters(IDynamicEditForm form, int id, object[] serviceParameters, Func<Task> dataChanged)
        {
            _dataChanged = dataChanged;
            _form = form;
            _id = id;
            _serviceParameters = serviceParameters;
            _dataProvider.SetParameters(_serviceParameters);
            NavigateFormatParameters = _id.GetFormatParameters(_serviceParameters);
            Fields = GetFields();
            ModelItem = GetItem();
            Buttons = _form.GetButtons();
            ButtonNavigations = _form.GetButtonNavigations();
        }

        public async Task<bool> Delete()
        {
            var success = true;

            try
            {
                Error = null;
                _dataProvider.DeleteItem(_form, ModelItem);
            }
            catch (Exception exc)
            {
                await LogException(exc);
                success = false;
            }

            return success;
        }

        public async Task<bool> Submit()
        {
            await Validate();
            var valid = !_validations.Any();

            if (valid)
            {
                try
                {
                    Error = null;
                    var pkField = Fields.FirstOrDefault(f => f.PrimaryKey);

                    if (GetFieldState(pkField).IsNew)
                    {
                        ModelItem = _dataProvider.InsertItem(_form, ModelItem);
                    }
                    else
                    {
                        _dataProvider.UpdateItem(_form, ModelItem);
                    }
                }
                catch(Exception exc)
                {
                    await LogException(exc);
                    valid = false;
                }
            }

            return valid;
        }

        private async Task LogException(Exception exc)
        {
            Error = exc.Message;

            if (exc.InnerException != null)
            {
                Error += "\r\n";
                Error += exc.InnerException.Message;
            }

            await _dataChanged?.Invoke();
        }

        public async Task Validate()
        {
            _validations = _dataValidationProvider.ValidateModel(_form, ModelItem, 0, Fields, _fieldStates);
            UpdateFieldStateValidations(_validations, 0);
        }

        public object GetItem()
        {
            bool isNew = false;
            var item = _dataProvider.GetItem(_form, _id);

            if (item == null)
            {
                item = CreateItem();
                isNew = true;
            }

            // Setup Primary Key
            var pkField = Fields.FirstOrDefault(f => f.PrimaryKey);

            if (pkField == null)
            {
                throw new SqlFormException($"Form '{_form.GetType().Name}' doesn't have PK");
            }

            var pkState = new FieldState(pkField) { IsEditing = true, IsNew = isNew };
            _fieldStates[pkField.BindingProperty] = pkState;

            _validations = _dataValidationProvider.ValidateCustomRules(_form, item, 0, Fields, FormRuleTriggers.Load);
            UpdateFieldStateValidations(_validations, 0);
            return item;
        }

        private object CreateItem()
        {
            var type = _form.GetEntityType();
            var item = Activator.CreateInstance(type);

            // Set Filter Key values
            item.SetFilterKeyValues(Fields, _serviceParameters);

            // run default validations
            _validations = _dataValidationProvider.ValidateCustomRules(_form, item, 0, Fields, FormRuleTriggers.Create);
            UpdateFieldStateValidations(_validations, 0);
            return item;
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

            //if (args.State.ValidationMessages.Any())
            {
                var validations = _dataValidationProvider.ValidateModelProperty(_form, ModelItem, args.State.RowIndex, args.Field.BindingProperty, Fields);
                UpdateFieldStateValidations(validations, args.State.RowIndex, args.Field.BindingProperty);
            }
        }

        private void UpdateFieldStateValidations(IEnumerable<ValidationResult> validations, int rowIndex, string bindingProperty = null)
        {
            if (bindingProperty == null)
            {
                ClearValidations();
            }
            else
            {
                _fieldStates[bindingProperty].ValidationMessages = new List<string>();
            }

            foreach (var validation in validations)
            {
                if (bindingProperty == null || validation.BindingProperty == bindingProperty)
                {
                    if (validation.ValidationResultType == ValidationResultTypes.Error)
                    {
                        _fieldStates[validation.BindingProperty].ValidationMessages.Add(validation.Message);
                    }
                }
            }
        }

        private void ClearValidations()
        {
            foreach (var state in _fieldStates.Values)
            {
                state.ValidationMessages = new List<string>();
            }
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
