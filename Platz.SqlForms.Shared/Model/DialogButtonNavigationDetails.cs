using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class DialogButtonNavigationDetails
    {
        public string LinkText { get; set; }
        public List<ButtonActionTypes> Actions { get; set; } = new List<ButtonActionTypes>();
    }
}
