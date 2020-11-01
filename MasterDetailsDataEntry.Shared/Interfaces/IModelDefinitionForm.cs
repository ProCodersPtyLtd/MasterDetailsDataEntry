using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("MasterDetailsDataEntry")]
[assembly: InternalsVisibleTo("MasterDetailsDataEntry.Tests")]
namespace MasterDetailsDataEntry.Shared
{
    public interface IModelDefinitionForm
    {
        Type GetDbContextType();
        Type GetDetailsType();
        internal IEnumerable<DataField> GetDetailsFields();
        string GetFieldFormat(string bindingProperty);
    }
}
