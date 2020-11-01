using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Platz.SqlForms
{
    public static class TypeExtensions
    {
        public static bool IsSimple(this Type t)
        {
            return t.IsPrimitive || t == typeof(DateTime) || t == typeof(DateTime?) || t == typeof(decimal) || t == typeof(decimal?)
                || t == typeof(string) || Nullable.GetUnderlyingType(t) != null;
        }

        public static bool IsList(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static IEnumerable<PropertyInfo> GetSimpleTypeProperties(this Type t)
        {
            var result = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType.IsSimple());
            return result;
        }
    }
}
