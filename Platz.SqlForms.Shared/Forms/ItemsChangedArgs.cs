using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class ItemsChangedArgs
    {
        public ItemOperations Operation { get; set; }
        public int RowIndex { get; set; }
    }
}
