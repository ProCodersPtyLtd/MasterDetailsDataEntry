using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public static class ObjectExtensions
    {
        public static object[] GetFormatParameters(this object first, object[] parameters)
        {
            var list = new List<object>();
            list.Add(first);

            if (parameters != null)
            {
                list.AddRange(parameters);
            }

            return list.ToArray();
        }
    }
}
