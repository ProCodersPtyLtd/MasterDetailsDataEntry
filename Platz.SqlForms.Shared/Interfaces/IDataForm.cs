using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms.Shared
{
    public interface IDataForm
    {
        Type GetDbContextType();
        string GetFieldFormat(DataField field);
    }
}
