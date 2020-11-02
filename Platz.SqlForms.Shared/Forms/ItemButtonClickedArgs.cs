using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class ItemButtonClickedArgs
    {
        public string Button { get; set; }
        public int RowIndex { get; set; }
        public object Item { get; set; }
        public int Pk { get; set; }
    }
}
