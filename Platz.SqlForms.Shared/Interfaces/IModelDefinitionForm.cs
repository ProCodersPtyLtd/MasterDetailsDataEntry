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
        Type GetMasterType();
        internal IEnumerable<DataField> GetDetailsFields();
        internal IEnumerable<DataField> GetMasterFields();
        internal IEnumerable<DataField> GetEntityFields(Type entity);
        string GetFieldFormat(DataField field);
    }
}
