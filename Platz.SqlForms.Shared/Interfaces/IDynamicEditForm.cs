using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public interface IDynamicEditForm
    {
        Type GetDbContextType();
        Type GetEntityType();
        IEnumerable<DataField> GetFields();
    }
}
