using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platz.SqlForms
{
    public class DataValidationProvider : IDataValidationProvider
    {
        public IEnumerable<ValidationResult> ValidateModel(object item, int rowIndex, IEnumerable<DataField> fields)
        {
            var result = new List<ValidationResult>();

            foreach (var field in fields)
            {
                RequiredRule(item, rowIndex, field, result);
            }

            return result;
        }

        public IEnumerable<ValidationResult> ValidateModelProperty(object item, int rowIndex, string bindingProperty, IEnumerable<DataField> fields)
        {
            var result = new List<ValidationResult>();
            var field = fields.Single(f => f.BindingProperty == bindingProperty);

            RequiredRule(item, rowIndex, field, result);
            
            return result;
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
    }
}
 