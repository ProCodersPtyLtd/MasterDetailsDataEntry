using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Platz.SqlForms")]
[assembly: InternalsVisibleTo("MasterDetailsDataEntry.Tests")]
namespace Platz.SqlForms.Shared
{
    public interface IModelDefinitionForm
    {
        Type GetDbContextType();
        Type GetDetailsType();
        internal IEnumerable<DataField> GetDetailsFields();
        string GetFieldFormat(string bindingProperty);
    }
}
