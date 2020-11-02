using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Platz.SqlForms.Shared
{
    public static class ObjectCloner
    {

        static ObjectCloner()
        {
        }

        public static object New(this object source)
        {
            var type = source.GetType();
            var target = Activator.CreateInstance(type);
            return target;
        }

        public static object GetCopy(this object source)
        {
            var target = New(source);
            source.CopyTo(target);
            return target;
        }

        public static void CopyTo(this object source, object target)
        {
            var properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                var value = property.GetValue(source);
                property.SetValue(target, value);
            }
        }
    }
}
