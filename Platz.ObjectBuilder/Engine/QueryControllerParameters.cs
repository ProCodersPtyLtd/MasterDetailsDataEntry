using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platz.ObjectBuilder
{
    public class QueryControllerParameters
    {
        public Type DbContextType { get; set; }
        public string SchemaFilename { get; set; }
    }
}
