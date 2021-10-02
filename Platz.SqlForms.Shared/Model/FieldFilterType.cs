using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public enum FieldFilterType
    {
        None,
        Text = 1,
        [Description("Text Starts")]
        TextStarts,
        [Description("Text Contains")]
        TextContains,
        TextEnds,
    }
}
