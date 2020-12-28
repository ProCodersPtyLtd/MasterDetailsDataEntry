using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class DataOperationArgs
    {
        public object[] Parameters { get; set; }
        public DataOperations Operation { get; set; }
        public DataOperationEvents OperationEvent { get; set; }
        public DbContext DbContext { get; set; }
    }

    public enum DataOperations
    {
        Insert,
        Update,
        Delete
    }

    public enum DataOperationEvents
    {
        AfterInsert
    }
}
