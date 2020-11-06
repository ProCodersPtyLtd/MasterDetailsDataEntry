using Platz.SqlForms.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms.Blazor
{
    public class RepeaterDataComponentController
    {
        public System.Collections.IList ModelItems => _modelItems;
        public IModelDefinitionForm Form => _form;
        public Type ModelType => _modelType;
        public IEnumerable<DataField> Fields => _fields;
        public FormViewOptions ViewOptions => _viewOptions;

        //[Parameter]
        private IModelDefinitionForm _form { get; set; }

        //[Parameter]
        private FormViewOptions _viewOptions { get; set; }

        //[Parameter]
        private Type _modelType { get; set; }

        //[Parameter]
        private IEnumerable<DataField> _fields { get; set; }

        //[Parameter]
        private System.Collections.IList _modelItems { get; set; }

        //[Parameter]
        //public EventCallback<ValueChangedArgs> ModelValueChanged { get; set; }

        ////[Parameter]
        //public EventCallback<ItemsChangedArgs> ItemsChanged { get; set; }

        ////[Parameter]
        //public EventCallback<ButtonClickedArgs> ButtonClicked { get; set; }



        private object _modelItemReserveCopy;
        public bool IsEditing { get; private set; }
        public string Error { get; private set; }
        private int _modelItemsHash;
        private IEnumerable<ValidationResult> _validations = new ValidationResult[0];
        private Dictionary<string, FieldState> _states = new Dictionary<string, FieldState>();

        private readonly IDataValidationProvider _dataValidationProvider;

        public RepeaterDataComponentController(IDataValidationProvider dataValidationProvider)
        {
            _dataValidationProvider = dataValidationProvider;
        }

        public void SetParameters(IModelDefinitionForm form, FormViewOptions viewOptions, Type modelType, IEnumerable<DataField> fields, IList modelItems)
        {
            _form = form;
            _viewOptions = viewOptions;
            _modelType = modelType;
            _fields = fields;
            _modelItems = modelItems;

            // check if items refreshed
            if (_modelItemsHash != ModelItems.GetHashCode())
            {
                IsEditing = false;
                _modelItemReserveCopy = null;
                _modelItemsHash = ModelItems.GetHashCode();
            }
        }

        public void InitControl()
        {
            if (ViewOptions?.StartNewEdititng == true)
            {
                AddNewItem();
            }
            else
            {
                IsEditing = false;
                _modelItemReserveCopy = null;
            }
        }

        public IEnumerable<DataField> GetFields()
        {
            return Fields.Where(f => !f.Button);
        }

        public IEnumerable<DataField> GetButtonFields()
        {
            return Fields.Where(f => f.Button);
        }

        #region events
        //private void RowMouseOver(MouseEventArgs args, int rowIndex)
        //{
        //    GetRowState(rowIndex).IsMouseOver = true;
        //}

        //private void RowMouseOut(MouseEventArgs args, int rowIndex)
        //{
        //    if (ModelItems.Count != 0)
        //    {
        //        GetRowState(rowIndex).IsMouseOver = false;
        //    }
        //}

        public void LocalModelValueChanged(ValueChangedArgs args)
        {
            SetItemValue(args.Field, args.State);

            if (args.State.ValidationMessages.Any())
            {
                var validations = _dataValidationProvider.ValidateModelProperty(Form, ModelItems[args.State.RowIndex], args.State.RowIndex, args.Field.BindingProperty, Fields);
                UpdateFieldStateValidations(validations, args.State.RowIndex, args.Field.BindingProperty);
            }

            // await ModelValueChanged.InvokeAsync(args);
        }
        #endregion

        public bool GetExtraHeight(int rowIndex)
        {
            return _validations.Any() && GetRowState(rowIndex).IsEditing;
        }

        #region field state

        public string GetFieldStateKey(string bindingProperty, int row)
        {
            string fieldStateKey = $"{bindingProperty}[{row}]";
            return fieldStateKey;
        }

        public FieldState CreateFieldState(DataField field, int row)
        {
            if (field.Button)
            {
                return null;
            }

            string fieldStateKey = GetFieldStateKey(field.BindingProperty, row);

            if (!_states.ContainsKey(fieldStateKey))
            {
                _states[fieldStateKey] = new FieldState(field, row);
            }

            var result = _states[fieldStateKey];
            result.Value = GetItemValue(field, row);
            result.RowIndex = row;
            return result;
        }

        public RowState GetRowState(int rowIndex)
        {
            return ModelItems[rowIndex].GetBag<RowState>();
        }
        #endregion

        #region reflection get set property
        public object GetItemValue(DataField field, int row)
        {
            var item = ModelItems[row];
            var result = item.GetPropertyValue(field.BindingProperty);
            return result;
        }

        public void SetItemValue(DataField field, FieldState state)
        {
            var item = ModelItems[state.RowIndex];
            item.SetPropertyValue(field.BindingProperty, state.Value);
        }
        #endregion

        #region clicks
        //private async Task CustomButtonClick(MouseEventArgs e, string button, int rowIndex)
        //{
        //    await ButtonClicked.InvokeAsync(new ButtonClickedArgs { Entity = ModelType, Button = button, RowIndex = rowIndex });
        //}

        public void EditItem(int rowIndex)
        {
            Error = null;
            IsEditing = true;
            ModelItems[rowIndex].GetBag<RowState>().IsEditing = true;
            _modelItemReserveCopy = ModelItems[rowIndex].GetCopy();
        }

        public async Task DeleteItem(int rowIndex, Func<ItemsChangedArgs, Task> asyncAction)
        {
            Error = null;
            var args = new ItemsChangedArgs { Entity = ModelType, Operation = ItemOperations.Delete, RowIndex = rowIndex };

            try
            {
                //await ItemsChanged.InvokeAsync(args);
                await asyncAction(args);
            }
            catch (Exception exc)
            {
                LogException(exc);
                // StateHasChanged();
            }
        }

        private void LogException(Exception exc)
        {
            Error = exc.Message;

            if (exc.InnerException != null)
            {
                Error += "\r\n";
                Error += exc.InnerException.Message;
            }
        }

        public async Task ApplyItem(int rowIndex, Func<ItemsChangedArgs, Task> asyncAction)
        {
            var editingFields = Fields.Where(f => CreateFieldState(f, rowIndex)?.IsEditing == true);
            _validations = _dataValidationProvider.ValidateModel(Form, ModelItems[rowIndex], rowIndex, editingFields);
            UpdateFieldStateValidations(_validations, rowIndex);

            if (_validations.Any())
            {
                return;
            }

            Error = null;
            var args = new ItemsChangedArgs { Entity = ModelType, RowIndex = rowIndex };
            args.Operation = ModelItems[rowIndex].GetBag<RowState>().IsNew ? ItemOperations.Add : ItemOperations.Update;

            try
            {
                // await ItemsChanged.InvokeAsync(args);
                await asyncAction(args);
            }
            catch (Exception exc)
            {
                LogException(exc);
                // StateHasChanged();
            }

            if (Error == null)
            {
                ModelItems[rowIndex].GetBag<RowState>().IsEditing = false;
                ModelItems[rowIndex].GetBag<RowState>().IsNew = false;
                IsEditing = false;
                SetPrimaryKeyIsNew(rowIndex, false);
            }
        }

        public void CancelItem(int rowIndex)
        {
            Error = null;
            ClearValidations();

            //if (!ModelItems[rowIndex].GetBag<RowState>().IsNew)
            //{
            //    _modelItemReserveCopy.CopyTo(ModelItems[rowIndex]);
            //}

            ModelItems[rowIndex].GetBag<RowState>().IsEditing = false;
            IsEditing = false;

            if (ModelItems[rowIndex].GetBag<RowState>().IsNew)
            {
                ModelItems.RemoveAt(rowIndex);
            }
            else
            {
                _modelItemReserveCopy.CopyTo(ModelItems[rowIndex]);
            }
        }

        public void AddNewItem()
        {
            Error = null;
            var rowIndex = ModelItems.Count;
            var item = Activator.CreateInstance(ModelType);
            ModelItems.Add(item);
            SetPrimaryKeyIsNew(rowIndex, true);


            EditItem(ModelItems.Count - 1);
            ModelItems[ModelItems.Count - 1].GetBag<RowState>().IsNew = true;
        }
        #endregion

        public void SetPrimaryKeyIsNew(int rowIndex, bool isNew)
        {
            var pkField = Fields.Single(f => f.PrimaryKey);
            CreateFieldState(pkField, rowIndex);
            var key = GetFieldStateKey(pkField.BindingProperty, rowIndex);
            _states[key].IsNew = isNew;
        }

        private void ClearValidations()
        {
            foreach (var state in _states.Values)
            {
                state.ValidationMessages = new List<string>();
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
                var key = GetFieldStateKey(bindingProperty, rowIndex);
                _states[key].ValidationMessages = new List<string>();
            }

            foreach (var validation in validations)
            {
                if (bindingProperty == null || validation.BindingProperty == bindingProperty)
                {
                    if (validation.ValidationResultType == ValidationResultTypes.Error)
                    {
                        var key = GetFieldStateKey(validation.BindingProperty, validation.RowIndex);
                        _states[key].ValidationMessages.Add(validation.Message);
                    }
                }
            }
        }
    }
}
