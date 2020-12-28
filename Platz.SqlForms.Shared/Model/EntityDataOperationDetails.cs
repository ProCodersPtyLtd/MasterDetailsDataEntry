using System;
using System.Collections.Generic;
using System.Text;

namespace Platz.SqlForms
{
    public class EntityDataOperationDetails
    {
        public Type EntityType { get; set; }
        public DataOperationEvents OperationEvent { get; set; }
        public Action<object, DataOperationArgs> Method { get; set; }
    }
}
