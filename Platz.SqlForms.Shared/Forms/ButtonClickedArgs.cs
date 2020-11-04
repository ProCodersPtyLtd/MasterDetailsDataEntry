using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class ButtonClickedArgs
    {
        public string Button { get; set; }
        public int RowIndex { get; set; }
        public Type Entity { get; set; }
    }
}
