using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public interface IDataValidationProvider
    {
        IEnumerable<ValidationResult> ValidateModel(object item, int rowIndex, IEnumerable<DataField> fields);
        IEnumerable<ValidationResult> ValidateModelProperty(object item, int rowIndex, string bindingProperty,  IEnumerable<DataField> fields);
    }
}
