using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public interface IModelDefinitionForm
    {
        Type GetDbContextType();
        Type GetDetailsType();
        IEnumerable<DataField> GetDetailsFields();
        string GetFieldFormat(string bindingProperty);
    }
}
