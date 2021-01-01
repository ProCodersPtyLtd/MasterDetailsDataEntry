using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class SqlFormException : Exception
    {
        public SqlFormException()
        {
        }

        public SqlFormException(string message) : base(message)
        {
        }
    }
}
