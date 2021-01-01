using Platz.SqlForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public interface IDataValidationProvider
    {
        IEnumerable<ValidationResult> ValidateCustomRules(IDataForm form, object item, int rowIndex, IEnumerable<DataField> fields, FormRuleTriggers trigger);
        IEnumerable<ValidationResult> ValidateModel(IDataForm form, object item, int rowIndex, IEnumerable<DataField> fields, Dictionary<string, FieldState> states = null);
        IEnumerable<ValidationResult> ValidateModelProperty(IDataForm form, object item, int rowIndex, string bindingProperty,  IEnumerable<DataField> fields);
    }
}
