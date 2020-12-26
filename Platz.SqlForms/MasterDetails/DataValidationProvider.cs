using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public class DataValidationProvider : IDataValidationProvider
    {
        private readonly IDataEntryProvider _dataEntryProvider;

        public DataValidationProvider(IDataEntryProvider dataEntryProvider)
        {
            _dataEntryProvider = dataEntryProvider;
        }

        public IEnumerable<ValidationResult> ValidateModel(IDataForm form, object item, int rowIndex, IEnumerable<DataField> fields)
        {
            var result = new List<ValidationResult>();

            foreach (var field in fields)
            {
                RequiredRule(item, rowIndex, field, result);
                UniqueRule(form, item, rowIndex, field, result);
                CustomRule(form, item, rowIndex, field, result);
            }

            return result;
        }

        public IEnumerable<ValidationResult> ValidateModelProperty(IDataForm form, object item, int rowIndex, string bindingProperty, IEnumerable<DataField> fields)
        {
            var result = new List<ValidationResult>();
            var field = fields.Single(f => f.BindingProperty == bindingProperty);

            RequiredRule(item, rowIndex, field, result);
            UniqueRule(form, item, rowIndex, field, result);
            CustomRule(form, item, rowIndex, field, result);
            
            return result;
        }

        private void CustomRule(IDataForm form, object item, int rowIndex, DataField field, List<ValidationResult> result)
        {
            foreach (var rule in field.Rules)
            {
                var vr = rule.Method(item);

                if (vr != null && vr.IsFailed)
                {
                    result.Add(new ValidationResult
                    {
                        Message = $"{vr.RuleName}: {vr.Message}",
                        BindingProperty = field.BindingProperty,
                        ValidationResultType = ValidationResultTypes.Error,
                        RowIndex = rowIndex
                    });
                }
            }
        }

        private void RequiredRule(object item, int rowIndex, DataField field, List<ValidationResult> result)
        {
            if (field.Required)
            {
                var value = item.GetPropertyValue(field.BindingProperty);

                if (value == null || value.ToString() == "" || (field.SelectEntityType != null && value.ToString() == "0"))
                {
                    result.Add(new ValidationResult
                    {
                        Message = "Required",
                        BindingProperty = field.BindingProperty,
                        ValidationResultType = ValidationResultTypes.Error,
                        RowIndex = rowIndex
                    });
                }
            }
        }

        private void UniqueRule(IDataForm form, object item, int rowIndex, DataField field, List<ValidationResult> result)
        {
            if (field.Unique)
            {
                if (_dataEntryProvider.IsPropertyValueNotUnique(form, item, field.BindingProperty, item.GetType()))
                {
                    result.Add(new ValidationResult
                    {
                        Message = "Not unique",
                        BindingProperty = field.BindingProperty,
                        ValidationResultType = ValidationResultTypes.Error,
                        RowIndex = rowIndex
                    });
                }
            }
        }
    }
}
 