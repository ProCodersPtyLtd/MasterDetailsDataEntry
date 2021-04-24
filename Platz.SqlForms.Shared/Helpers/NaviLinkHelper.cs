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
