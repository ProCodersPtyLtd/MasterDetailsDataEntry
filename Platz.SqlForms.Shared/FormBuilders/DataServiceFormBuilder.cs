using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Platz.SqlForms
{
    public class DataServiceFormBuilder : FormBuilder
    {
        public Func<object[], object> ListQuery { get; private set; }
        public Func<QueryOptions, object[], object> ListQueryOptions { get; private set; }

        public virtual void SetListMethod(Func<object[], object> method)
        {
            ListQuery = method;
        }
        public virtual void SetListMethod(Func<QueryOptions, object[], object> method)
        {
            ListQueryOptions = method;
        }
    }
}
