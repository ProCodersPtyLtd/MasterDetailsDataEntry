using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.ObjectBuilder.Blazor
{
    public class TableObj
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        public List<StoreQueryField> Fields { get; set; } = new List<StoreQueryField>();
    }

    public class StoreQueryField
    {
        // Unique Field Alias in Query = FieldName by Default
        public string Field { get; set; }
        public string FieldAlias { get; set; }
        public bool Checked { get; set; }
    }

}
