using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.SqlForms.Shared
{
    public static class StringExtensions
    {
        public static string TrimName(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            var first = true;

            foreach (char c in source)
            {
                if ((c >= '0' && c <= '9') && first)
                {
                    continue;
                }    

                if ( (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                {
                    sb.Append(c);
                }

                first = false;
            }

            return sb.ToString();
        }
    }
}
