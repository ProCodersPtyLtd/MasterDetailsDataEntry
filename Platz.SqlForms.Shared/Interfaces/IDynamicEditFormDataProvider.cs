using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public interface IDynamicEditFormDataProvider
    {
        IEnumerable<DataField> GetFormFields(IDynamicEditForm form);
    }
}
