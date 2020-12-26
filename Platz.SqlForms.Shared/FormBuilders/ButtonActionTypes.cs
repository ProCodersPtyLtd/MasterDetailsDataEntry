using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public enum ButtonActionTypes
    {
        Submit = 1, // OK navigation
        Delete,     // OK navigation
        Cancel,     // Reject navigation
        Close,
        Validate,
        Custom
    }
}
