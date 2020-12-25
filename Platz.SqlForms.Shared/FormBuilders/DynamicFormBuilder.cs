using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Platz.SqlForms
{
    public class DynamicFormBuilder : FormBuilder
    {
        public Func<object[], object> ListQuery { get; private set; }

        public virtual void SetListMethod(Func<object[], object> method)
        {
            ListQuery = method;
        }
    }
}
