using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MasterDetailsDataEntry.Shared
{
    public static class ObjectBag
    {
        private static Dictionary<object, object> _bags = new Dictionary<object, object>();

        public static void AddBag<T>(this object target, T bag)
            where T : class, new()
        {
            _bags[target] = bag;
        }

        public static T GetBag<T>(this object target)
            where T: class, new()
        {
            if (!_bags.ContainsKey(target))
            {
                _bags[target] = new T();
            }

            return _bags[target] as T;
        }
    }
}
