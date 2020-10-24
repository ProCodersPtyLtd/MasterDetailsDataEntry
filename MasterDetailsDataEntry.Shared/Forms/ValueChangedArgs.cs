using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared.Forms
{
    public class ValueChangedArgs
    {
        public DataField Field { get; set; }
        public FieldState State { get; set; }
        public object NewValue { get; set; }
        public int RowIndex { get; set; }
    }
}
