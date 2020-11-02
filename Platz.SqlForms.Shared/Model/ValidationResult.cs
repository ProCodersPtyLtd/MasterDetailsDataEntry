using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class ValidationResult
    {
        public string BindingProperty { get; set; }
        public string Message { get; set; }
        public int RowIndex { get; set; }

        public ValidationResultTypes ValidationResultType { get; set; }
    }
}
