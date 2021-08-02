using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public enum FieldFilterType
    {
        None,
        Text,
        TextStarts,
        TextContains,
        TextEnds,
        NumExpression,
        Select
    }
}
