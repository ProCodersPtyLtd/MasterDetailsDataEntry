using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms.Blazor
{
    public class DynamicEditController
    {
        public string Error { get; private set; }

        public IEnumerable<DataField> GetFields()
        {
            return null;
        }
    }
}
