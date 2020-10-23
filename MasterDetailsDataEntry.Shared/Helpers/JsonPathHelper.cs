using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailsDataEntry.Shared.Helpers
{
    public static class JsonPathHelper
    {
        public static string ReplaceLambdaVar(this string selectorString)
        {
            var result = selectorString.Trim();
            var values = result.Split('.');

            if (values.Length > 0)
            {
                result = result.Substring(values[0].Length + 1);
            }

            return result;
        }
    }
}
