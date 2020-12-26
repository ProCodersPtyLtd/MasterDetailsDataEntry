using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Platz.SqlForms")]
[assembly: InternalsVisibleTo("MasterDetailsDataEntry.Tests")]
namespace Platz.SqlForms.Shared
{
    public interface IDynamicEditForm : IDataForm
    {
        Type GetEntityType();
        internal IEnumerable<DataField> GetFields();
        IEnumerable<DialogButtonDetails> GetButtons();
    }
}
