using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Platz.ObjectBuilder.Blazor.Controllers.Validation
{
    public static class StackTraceHelper
    {
        public static Type GetCallingClass()
        {
            StackTrace st = new StackTrace();
            var sf = st.GetFrames().Skip(2).FirstOrDefault();
            var result = sf.GetMethod().ReflectedType;
            return result;
        }
    }
}
