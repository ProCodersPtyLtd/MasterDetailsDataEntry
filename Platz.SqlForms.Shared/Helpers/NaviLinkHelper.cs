using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms
{
    public static class NaviLinkHelper
    {
        public static string GetLink(string pattern, object[] parameters)
        {
            try
            {
                return string.Format(pattern, parameters);
            }
            catch
            {
                var p = string.Join(',', parameters);
                throw new NavigationLinkException($"Link pattern '{pattern}' doesn't correspond to supplied parameters: {p}");
            }
        }

        public static string GetLink(string pattern, FormParameter[] parameters)
        {
            var result = pattern;

            foreach (var p in parameters)
            {
                var mark = $"{{{p.Name}}}";
                // ToDo: add data type support to convert p.Value to String in a proper way
                result = result.Replace(mark, p.Value.ToString());
            }

            return result;
        }
    }

    public class NavigationLinkException : Exception
    {
        public NavigationLinkException()
        {
        }

        public NavigationLinkException(string message) : base(message)
        {
        }
    }
}
