using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared.Forms
{
    public class ItemsChangedArgs
    {
        public ItemOperations Operation { get; set; }
        public int RowIndex { get; set; }
    }
}
