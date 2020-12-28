using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class ButtonActionArgs
    {
        public ButtonActionTypes ActionType { get; set; }

        public string Button { get; set; }
        public int RowIndex { get; set; }
        public Type Entity { get; set; }
    }
}
